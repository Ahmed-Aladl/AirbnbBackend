using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.Middleware
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
