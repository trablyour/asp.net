namespace JwtChat.Services
{
    // кто сейчас в чате; у пользователя может быть несколько вкладок
    public class ConnectionTracker
    {
        private readonly Dictionary<string, HashSet<string>> _users = new();
        private readonly object _lock = new();

        public bool Add(string userName, string connectionId)
        {
            lock (_lock)
            {
                if (!_users.TryGetValue(userName, out var connections))
                {
                    connections = new HashSet<string>();
                    _users[userName] = connections;
                }

                connections.Add(connectionId);
                return connections.Count == 1;
            }
        }

        public bool Remove(string userName, string connectionId)
        {
            lock (_lock)
            {
                if (!_users.TryGetValue(userName, out var connections))
                    return false;

                connections.Remove(connectionId);
                if (connections.Count > 0)
                    return false;

                _users.Remove(userName);
                return true;
            }
        }

        public List<string> OnlineUsers()
        {
            lock (_lock)
            {
                return _users.Keys.OrderBy(u => u).ToList();
            }
        }
    }
}
