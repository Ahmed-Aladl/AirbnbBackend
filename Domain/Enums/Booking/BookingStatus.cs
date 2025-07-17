using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums.Booking
{
    public enum BookingStatus
    {
        Pending,

        Confirmed, //: The booking is secured. Payment has been made, and all conditions have been met.

        Cancelled, //: The booking has been cancelled, either by the user or the owner, before the check-in date.

        Completed, //: The guest has successfully checked out, and the booking is considered finished.

        NoShow, //: The guest never arrived for their confirmed booking.

        Failed, //: The booking attempt failed, usually due to a payment processing error.

        AwaitingPayment, //: The booking is reserved for a short period, waiting for the user to complete the payment.

    }
}
