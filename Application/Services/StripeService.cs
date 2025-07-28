using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Enums.Payment;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace Application.Services
{
    public interface IStripeService
    {
        Task<string> CreateCheckoutSessionAsync(int bookingId, int amount);
        Task HandleWebhookAsync(HttpRequest request);
        Task<string> CreateExpressAccountAsync(string email, string hostId);
        Task<bool> TransferToHostAsync(int paymentId);
        Task<bool> EnableTransfersCapabilityAsync(string hostId);
        Task ProcessPendingTransfersForHostAsync(string hostId); 
    }

    public class StripeService : IStripeService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StripeService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly NotificationService _notificationService;
        private const decimal PLATFORM_FEE_PERCENTAGE = 0.10m; // 10%

        public StripeService(IConfiguration config, IUnitOfWork unitOfWork, ILogger<StripeService> logger,
            UserManager<User> userManager, NotificationService notificationService)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
            _notificationService = notificationService;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSessionAsync(int bookingId, int amount)
        {
            try
            {
                _logger.LogInformation($"Creating Stripe session for booking {bookingId}, amount {amount}");

                var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
                if (booking == null)
                    throw new Exception($"Booking {bookingId} not found");

                // Calculate platform fee and host amount
                var platformFee = (int)(amount * PLATFORM_FEE_PERCENTAGE);
                var hostAmount = amount - platformFee;

                var sessionService = new SessionService();
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new()
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                UnitAmount = amount,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Booking #{bookingId}"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = "http://localhost:4200/",
                    CancelUrl = "http://localhost:4200/",
                    Metadata = new Dictionary<string, string>
                    {
                        { "booking_id", bookingId.ToString() }
                    }
                };

                var session = await sessionService.CreateAsync(options);
                _logger.LogInformation($"Stripe session created successfully: {session.Id}");

                // Save payment in database with calculated amounts
                var payment = new Payment
                {
                    Amount = amount,
                    PlatformFee = platformFee,
                    HostAmount = hostAmount,
                    BookingId = bookingId,
                    StripeSessionId = session.Id,
                    Status = PaymentStatus.Pending,
                    TransferStatus = TransferStatus.NotTransferred,
                    Currency = "usd",
                    PaymentDate = DateTime.UtcNow,
                    UserId = booking.UserId
                };

                _logger.LogInformation($"Adding payment to database: BookingId={payment.BookingId}, Amount={payment.Amount}, PlatformFee={payment.PlatformFee}, HostAmount={payment.HostAmount}");

                _unitOfWork.paymentRepository.Add(payment);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation($"Payment saved successfully with ID: {payment.Id}");

                return session.Url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCheckoutSessionAsync");
                throw;
            }
        }

        public async Task HandleWebhookAsync(HttpRequest request)
        {
            try
            {
                _logger.LogInformation($"Webhook handling started - Method: {request.Method}, Path: {request.Path}");

                if (!request.Headers.ContainsKey("Stripe-Signature"))
                {
                    _logger.LogWarning("Missing Stripe-Signature header");
                    throw new Exception("Missing Stripe-Signature header");
                }

                request.EnableBuffering();
                request.Body.Position = 0;

                string json;
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    json = await reader.ReadToEndAsync();
                }

                if (string.IsNullOrEmpty(json))
                {
                    _logger.LogWarning("Empty webhook payload received");
                    throw new Exception("Empty webhook payload");
                }

                request.Body.Position = 0;

                var secret = _config["Stripe:WebhookSecret"];
                if (string.IsNullOrEmpty(secret))
                {
                    _logger.LogError("Stripe webhook secret is not configured");
                    throw new Exception("Stripe webhook secret is not configured");
                }

                Event stripeEvent;
                try
                {
                    stripeEvent = EventUtility.ConstructEvent(
                        json,
                        request.Headers["Stripe-Signature"].ToString(),
                        secret
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Stripe webhook signature verification failed");
                    throw new Exception("Stripe webhook signature verification failed.", ex);
                }

                _logger.LogInformation($"Successfully parsed webhook event: {stripeEvent.Type} (ID: {stripeEvent.Id})");

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        await HandleCheckoutSessionCompleted(stripeEvent);
                        break;
                    case "checkout.session.failed":
                        await HandleCheckoutSessionFailed(stripeEvent);
                        break;
                    case "checkout.session.canceled":
                        await HandleCheckoutSessionCanceled(stripeEvent);
                        break;
                    case "account.updated":
                        await HandleAccountUpdated(stripeEvent);
                        break;
                    default:
                        _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                _logger.LogInformation("Webhook handled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HandleWebhookAsync: {Message}", ex.Message);
                throw;
            }
        }

        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            try
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null) return;

                _logger.LogInformation($"Processing checkout.session.completed for session: {session.Id}");

                var payment = await _unitOfWork.paymentRepository.GetBySessionIdAsync(session.Id);
                if (payment != null && session.PaymentIntentId != null)
                {
                    payment.StripePaymentIntentId = session.PaymentIntentId;
                    payment.Status = PaymentStatus.Succeeded;

                    var booking = await _unitOfWork.Bookings.GetBookingWithPropertyAsync(payment.BookingId);
                    if (booking != null)
                    {
                        booking.BookingStatus = Domain.Enums.Booking.BookingStatus.Confirmed;
                        _logger.LogInformation($"Booking {booking.Id} status updated to Confirmed.");

                        // Send notifications
                        await SendPaymentNotifications(booking, payment);

                        // Try to transfer to host if they have a complete Stripe account
                        if (booking.Property?.HostId != null)
                        {
                            var hostId = booking.Property.HostId.ToString();
                            var host = await _userManager.FindByIdAsync(hostId);

                            if (host != null && !string.IsNullOrEmpty(host.StripeAccountId))
                            {
                                // Check if account is complete and can receive transfers
                                if (await IsAccountReadyForTransfers(host.StripeAccountId))
                                {
                                    await TransferToHostAsync(payment.Id);
                                }
                                else
                                {
                                    payment.TransferStatus = TransferStatus.PendingTransfer;
                                    _logger.LogInformation($"Host account {host.StripeAccountId} not ready for transfers. Payment marked as pending transfer.");
                                }
                            }
                            else
                            {
                                payment.TransferStatus = TransferStatus.PendingTransfer;
                                _logger.LogInformation($"Host doesn't have Stripe account. Payment marked as pending transfer.");
                            }
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Updated payment with PaymentIntentId: {session.PaymentIntentId}");
                }
                else
                {
                    _logger.LogWarning($"Payment not found for session: {session.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling checkout.session.completed");
                throw;
            }
        }

        private async Task HandleCheckoutSessionFailed(Event stripeEvent)
        {
            try
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null) return;

                _logger.LogInformation($"Processing checkout.session.failed for session: {session.Id}");

                var payment = await _unitOfWork.paymentRepository.GetBySessionIdAsync(session.Id);
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = "Checkout session failed";
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Updated payment status to 'Failed' for session: {session.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling checkout.session.failed");
                throw;
            }
        }

        private async Task HandleCheckoutSessionCanceled(Event stripeEvent)
        {
            try
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null) return;

                _logger.LogInformation($"Processing checkout.session.canceled for session: {session.Id}");

                var payment = await _unitOfWork.paymentRepository.GetBySessionIdAsync(session.Id);
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Canceled;
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Updated payment status to 'Canceled' for session: {session.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling checkout.session.canceled");
                throw;
            }
        }

        // New method to handle account updates and auto-transfer pending payments
        private async Task HandleAccountUpdated(Event stripeEvent)
        {
            try
            {
                var account = stripeEvent.Data.Object as Account;
                if (account == null) return;

                _logger.LogInformation($"Processing account.updated for account: {account.Id}");

                // Find the host with this Stripe account
                var host = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.StripeAccountId == account.Id);

                if (host != null && await IsAccountReadyForTransfers(account.Id))
                {
                    _logger.LogInformation($"Account {account.Id} is now ready for transfers. Processing pending transfers for host {host.Id}");
                    await ProcessPendingTransfersForHostAsync(host.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling account.updated");
                throw;
            }
        }

        public async Task<string> CreateExpressAccountAsync(string email, string hostId)
        {
            try
            {
                var accountService = new AccountService();
                var account = await accountService.CreateAsync(new AccountCreateOptions
                {
                    Type = "express",
                    Email = email,
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                    },
                    BusinessType = "individual"
                });

                var host = await _userManager.FindByIdAsync(hostId);
                if (host != null)
                {
                    host.StripeAccountId = account.Id;
                    await _userManager.UpdateAsync(host);
                }

                var linkService = new AccountLinkService();
                var link = await linkService.CreateAsync(new AccountLinkCreateOptions
                {
                    Account = account.Id,
                    RefreshUrl = "http://localhost:4200/hostsettings/Wallet",
                    ReturnUrl = "http://localhost:4200/hostsettings/Wallet",
                    Type = "account_onboarding"
                });

                return link.Url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating express account");
                throw;
            }
        }

        public async Task<bool> TransferToHostAsync(int paymentId)
        {
            try
            {
                var payment = await _unitOfWork.paymentRepository.GetByIdAsync(paymentId);
                if (payment == null || payment.Status != PaymentStatus.Succeeded)
                {
                    _logger.LogWarning($"Payment {paymentId} not found or not succeeded");
                    return false;
                }

                if (payment.TransferStatus == TransferStatus.Transferred)
                {
                    _logger.LogInformation($"Payment {paymentId} already transferred");
                    return true;
                }

                var booking = await _unitOfWork.Bookings.GetBookingWithPropertyAsync(payment.BookingId);
                if (booking?.Property?.HostId == null)
                {
                    _logger.LogWarning($"Booking or property not found for payment {paymentId}");
                    return false;
                }

                var host = await _userManager.FindByIdAsync(booking.Property.HostId.ToString());
                if (host?.StripeAccountId == null)
                {
                    _logger.LogWarning($"Host {booking.Property.HostId} doesn't have Stripe account");
                    return false;
                }

                if (!await IsAccountReadyForTransfers(host.StripeAccountId))
                {
                    _logger.LogWarning($"Host account {host.StripeAccountId} not ready for transfers");
                    payment.TransferStatus = TransferStatus.PendingTransfer;
                    await _unitOfWork.SaveChangesAsync();
                    return false;
                }

                payment.TransferStatus = TransferStatus.PendingTransfer;
                await _unitOfWork.SaveChangesAsync();

                var transferService = new TransferService();
                var transfer = await transferService.CreateAsync(new TransferCreateOptions
                {
                    Amount = (long)(payment.HostAmount * 100), // Use calculated host amount
                    Currency = "usd",
                    Destination = host.StripeAccountId,
                    Description = $"Transfer for booking {booking.Id}",
                    Metadata = new Dictionary<string, string>
                    {
                        { "payment_id", payment.Id.ToString() },
                        { "booking_id", booking.Id.ToString() }
                    }
                });

                payment.TransferStatus = TransferStatus.Transferred;
                payment.StripeTransferId = transfer.Id;
                payment.TransferDate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Successfully transferred {payment.HostAmount} to host {host.Id} for payment {payment.Id}");

                // Send notification to host
                await _notificationService.SendNotification(new Domain.Models.Notification
                {
                    Message = $"Payment of ${payment.HostAmount:F2} has been transferred to your account for booking #{booking.Id}.",
                    CreatedAt = DateTime.UtcNow,
                    isRead = false,
                    UserId = host.Id
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error transferring payment {paymentId} to host");

                // Update payment status to transfer failed
                var payment = await _unitOfWork.paymentRepository.GetByIdAsync(paymentId);
                if (payment != null)
                {
                    payment.TransferStatus = TransferStatus.TransferFailed;
                    payment.FailureReason = ex.Message;
                    await _unitOfWork.SaveChangesAsync();
                }

                return false;
            }
        }

        public async Task ProcessPendingTransfersForHostAsync(string hostId)
        {
            try
            {
                var pendingPayments = await _unitOfWork.paymentRepository.GetPendingPaymentsForHostAsync(hostId);

                _logger.LogInformation($"Found {pendingPayments.Count} pending transfers for host {hostId}");

                foreach (var payment in pendingPayments)
                {
                    await TransferToHostAsync(payment.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing pending transfers for host {hostId}");
                throw;
            }
        }

        private async Task<bool> IsAccountReadyForTransfers(string stripeAccountId)
        {
            try
            {
                var accountService = new AccountService();
                var account = await accountService.GetAsync(stripeAccountId);

                return account.Capabilities?.Transfers == "active" &&
                       account.ChargesEnabled &&
                       account.PayoutsEnabled;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking account readiness for {stripeAccountId}");
                return false;
            }
        }

        private async Task SendPaymentNotifications(Booking booking, Payment payment)
        {
            try
            {
                // Guest notification
                await _notificationService.SendNotification(new Domain.Models.Notification
                {
                    Message = $"Your payment of ${payment.Amount / 100:F2} for booking #{booking.Id} was successful.",
                    CreatedAt = DateTime.UtcNow,
                    isRead = false,
                    UserId = booking.UserId
                });

                // Host notification
                if (booking.Property?.HostId != null)
                {
                    var host = await _userManager.FindByIdAsync(booking.Property.HostId.ToString());
                    string hostMessage = !string.IsNullOrEmpty(host?.StripeAccountId)
                        ? $"You have received a new payment of ${payment.HostAmount / 100:F2} for booking #{booking.Id}."
                        : $"You have a new payment of ${payment.HostAmount / 100:F2} for booking #{booking.Id}. Please complete your Stripe account setup to receive it.";

                    await _notificationService.SendNotification(new Domain.Models.Notification
                    {
                        Message = hostMessage,
                        CreatedAt = DateTime.UtcNow,
                        isRead = false,
                        UserId = booking.Property.HostId.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending payment notifications");
            }
        }

        public async Task<bool> EnableTransfersCapabilityAsync(string hostId)
        {
            try
            {
                var host = await _userManager.FindByIdAsync(hostId);
                if (host?.StripeAccountId == null)
                    return false;

                var accountService = new AccountService();
                var updateOptions = new AccountUpdateOptions
                {
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        Transfers = new AccountCapabilitiesTransfersOptions
                        {
                            Requested = true
                        }
                    }
                };

                await accountService.UpdateAsync(host.StripeAccountId, updateOptions);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enabling transfers capability for host {hostId}");
                return false;
            }
        }
    }
}