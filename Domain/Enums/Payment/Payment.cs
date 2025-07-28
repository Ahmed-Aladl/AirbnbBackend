using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums.Payment
{
    public enum PaymentStatus
    {
        Pending = 1,
        Processing = 2,
        Succeeded = 3,
        Failed = 4,
        Canceled = 5,
        Refunded = 6,
        RequiresAction = 7
    }

    public enum TransferStatus
    {
        NotTransferred = 1,
        PendingTransfer = 2,
        Transferred = 3,
        TransferFailed = 4
    }
}
