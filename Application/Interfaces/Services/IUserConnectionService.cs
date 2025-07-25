using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserConnectionService
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId, string connectionId);
        IEnumerable<string> GetConnections(string userId);
    }
}
