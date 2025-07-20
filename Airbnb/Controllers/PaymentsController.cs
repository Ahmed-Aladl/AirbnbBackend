using Application.DTOs.AmenityDTOs;
using Application.DTOs.PaymentDTOs;
using Application.Result;
using Application.Services;
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

                var url = await _stripeService.CreateCheckoutSessionAsync(dto.BookingId, dto.Amount);

                return Ok(new { url, success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return StatusCode(500, new
                {
                    error = "Failed to create checkout session",
                    details = ex.Message
                });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                await _stripeService.HandleWebhookAsync(Request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing failed");
                return BadRequest();
            }
        }

        //[HttpPost("intent")]
        //public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentDTO dto)
        //{
        //    try
        //    {
        //        var result = await _paymentService.CreatePaymentIntentAsync(dto);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating payment intent");
        //        return StatusCode(500, new
        //        {
        //            error = "Failed to create payment intent",
        //            details = ex.Message
        //        });
        //    }
        //}


    }
}