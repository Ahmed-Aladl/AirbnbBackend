using Application.DTOs.PaymentDTOs;
using Application.Services;
using Domain.Enums.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Airbnb.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IStripeService stripeService,
            PaymentService paymentService,
            ILogger<PaymentsController> logger)
        {
            _stripeService = stripeService;
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreatePaymentDTO dto)
        {
            try
            {
                _logger.LogInformation($"Creating checkout session for booking {dto.BookingId}");

                var url = await _stripeService.CreateCheckoutSessionAsync(dto.BookingId);

                return Ok(new { url, success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to create checkout session",
                    details = ex.Message
                });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            _logger.LogInformation("⚠️ Stripe webhook called.");

            try
            {
                await _stripeService.HandleWebhookAsync(Request);
                _logger.LogInformation("✅ Webhook handled successfully.");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing failed");
                return BadRequest();
            }
        }

        [HttpPost("finalize-account")]
        [Authorize]
        public async Task<IActionResult> FinalizeStripeAccount([FromQuery] string email, [FromQuery] string userId)
        {
            try
            {
                var url = await _stripeService.CreateExpressAccountAsync(email, userId);
                return Ok(new { success = true, url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing Stripe account");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("transfer/{paymentId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> TransferToHost(int paymentId)
        {
            try
            {
                var success = await _stripeService.TransferToHostAsync(paymentId);
                if (success)
                    return Ok(new { success = true, message = "Transfer completed successfully" });

                return BadRequest(new { success = false, message = "Transfer failed. Check payment or host data." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring payment to host");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("host/{hostId}/pending")]
        [Authorize]
        public async Task<IActionResult> GetPendingPayments(string hostId)
        {
            try
            {
                var payments = await _paymentService.GetPendingPaymentsForHostAsync(hostId);
                return Ok(new { success = true, payments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending payments");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("host/{hostId}/summary")]
        [Authorize]
        public async Task<IActionResult> GetPaymentSummary(string hostId)
        {
            try
            {
                var summary = await _paymentService.GetPaymentSummaryAsync(hostId);
                return Ok(new { success = true, summary });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment summary");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("host/{hostId}")]
        [Authorize]
        public async Task<IActionResult> GetHostPayments(string hostId, [FromQuery] PaymentStatus? status = null)
        {
            try
            {
                var payments = await _paymentService.GetHostPaymentsAsync(hostId, status);
                return Ok(new { success = true, payments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching host payments");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("process-pending-transfers")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> ProcessPendingTransfers()
        {
            try
            {
                var pendingPayments = await _paymentService.GetPaymentsByStatusAsync(PaymentStatus.Succeeded);
                var pendingTransfers = pendingPayments.Where(p => p.TransferStatus == TransferStatus.PendingTransfer).ToList();

                var processedCount = 0;
                foreach (var payment in pendingTransfers)
                {
                    if (await _stripeService.TransferToHostAsync(payment.Id))
                        processedCount++;
                }

                return Ok(new
                {
                    success = true,
                    message = $"Processed {processedCount} out of {pendingTransfers.Count} pending transfers"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending transfers");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("admin/revenue")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPlatformRevenue([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var revenue = await _paymentService.GetPlatformRevenueAsync(startDate, endDate);
                return Ok(new { success = true, revenue = revenue / 100m }); // Convert from cents to dollars
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching platform revenue");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("booking/{bookingId}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentByBooking(int bookingId)
        {
            try
            {
                var payment = await _paymentService.GetPaymentByBookingIdAsync(bookingId);
                if (payment == null)
                    return NotFound(new { success = false, message = "Payment not found" });

                return Ok(new { success = true, payment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment by booking");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("payments")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _paymentService.GetAllPaymentsForAdminAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("stripe-account-completed")]
        public async Task<IActionResult> IsStripeAccountCompleted([FromQuery] string userId)
        {
            var result = await _paymentService.IsStripeAccountCompletedAsync(userId);
            return Ok(new { accountCompleted = result });
        }

    }
}