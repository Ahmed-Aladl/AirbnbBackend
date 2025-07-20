using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
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
    }

    public class StripeService : IStripeService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StripeService> _logger;

        public StripeService(IConfiguration config, IUnitOfWork unitOfWork, ILogger<StripeService> logger)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _logger = logger;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSessionAsync(int bookingId, int amount)
        {
            try
            {
                _logger.LogInformation($"Creating Stripe session for booking {bookingId}, amount {amount}");

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
                    SuccessUrl = "https://www.airbnb.com/",
                    CancelUrl = "https://www.airbnb.com/experiences",
                    Metadata = new Dictionary<string, string>
                    {
                        { "booking_id", bookingId.ToString() }
                    }
                };

                var session = await sessionService.CreateAsync(options);
                _logger.LogInformation($"Stripe session created successfully: {session.Id}");

                // Save payment in database
                var payment = new Payment
                {
                    Amount = amount,
                    BookingId = bookingId,
                    StripeSessionId = session.Id,
                    Status = "Pending",
                    Currency = "usd",
                    PaymentDate = DateTime.UtcNow
                };

                _logger.LogInformation($"Adding payment to database: BookingId={payment.BookingId}, Amount={payment.Amount}");

                try
                {
                    _unitOfWork.paymentRepository.Add(payment);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Payment saved successfully with ID: {payment.Id}");
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database error while saving payment");
                    throw new Exception($"Database error: {dbEx.Message}", dbEx);
                }

                return session.Url;
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe API error");
                throw new Exception($"Stripe error: {stripeEx.Message}", stripeEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error in CreateCheckoutSessionAsync");
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

                _logger.LogInformation($"Webhook payload length: {json.Length}");
                _logger.LogInformation($"First 100 chars: {json.Substring(0, Math.Min(100, json.Length))}");

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

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    await HandleCheckoutSessionCompleted(stripeEvent);
                }
                else if (stripeEvent.Type == "checkout.session.failed")
                {
                    await HandleCheckoutSessionFailed(stripeEvent);
                }
                else if (stripeEvent.Type == "checkout.session.canceled")
                {
                    await HandleCheckoutSessionCanceled(stripeEvent);
                }
                else
                {
                    _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
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
                if (session != null)
                {
                    _logger.LogInformation($"Processing checkout.session.completed for session: {session.Id}");

                    var payment = await _unitOfWork.paymentRepository.GetBySessionIdAsync(session.Id);
                    if (payment != null && session.PaymentIntentId != null)
                    {
                        payment.StripePaymentIntentId = session.PaymentIntentId;
                        payment.Status = "Succeeded";
                        await _unitOfWork.SaveChangesAsync();
                        _logger.LogInformation($"Updated payment with PaymentIntentId: {session.PaymentIntentId}");
                    }
                    else
                    {
                        _logger.LogWarning($"Payment not found for session: {session.Id}");
                    }
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
                if (session != null)
                {
                    _logger.LogInformation($"Processing checkout.session.failed for session: {session.Id}");

                    var payment = await _unitOfWork.paymentRepository.GetBySessionIdAsync(session.Id);
                    if (payment != null)
                    {
                        payment.Status = "Failed";
                        await _unitOfWork.SaveChangesAsync();
                        _logger.LogInformation($"Updated payment status to 'Failed' for session: {session.Id}");
                    }
                    else
                    {
                        _logger.LogWarning($"Payment not found for failed session: {session.Id}");
                    }
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
                if (session != null)
                {
                    _logger.LogInformation($"Processing checkout.session.canceled for session: {session.Id}");

                    var payment = await _unitOfWork.paymentRepository.GetBySessionIdAsync(session.Id);
                    if (payment != null)
                    {
                        payment.Status = "Canceled";
                        await _unitOfWork.SaveChangesAsync();
                        _logger.LogInformation($"Updated payment status to 'Canceled' for session: {session.Id}");
                    }
                    else
                    {
                        _logger.LogWarning($"Payment not found for canceled session: {session.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling checkout.session.canceled");
                throw;
            }
        }
    }
}
