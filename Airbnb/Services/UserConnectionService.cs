namespace Airbnb.Services
{
    using System.Collections.Concurrent;
    using Application.Interfaces.Services;

    public class UserConnectionService : IUserConnectionService
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connections = new();

        public void AddConnection(string userId, string connectionId)
        {
            _connections.AddOrUpdate(userId,
                _ => new HashSet<string> { connectionId },
                (_, set) =>
                {
                    lock (set)
                    {
                        set.Add(connectionId);
                        return set;
                    }
                });
        }

        public void RemoveConnection(string userId, string connectionId)
        {
            if (_connections.TryGetValue(userId, out var set))
            {
                lock (set)
                {
                    set.Remove(connectionId);
                    if (set.Count == 0)
                    {
                        _connections.TryRemove(userId, out _);
                    }
                }
            }
        }

        public IEnumerable<string> GetConnections(string userId)
        {
            return _connections.TryGetValue(userId, out var set)
                ? set
                : Enumerable.Empty<string>();
        }
    }

}
