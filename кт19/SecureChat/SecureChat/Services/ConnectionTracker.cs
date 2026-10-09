namespace SecureChat.Services
{
    // кто сейчас в чате; у одного пользователя может быть несколько вкладок (подключений)
    public class ConnectionTracker
    {
        private readonly Dictionary<string, HashSet<string>> _users = new();
        private readonly object _lock = new();

        // true, если это первое подключение пользователя (он только что вошел)
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

        // true, если закрылось последнее подключение (пользователь вышел)
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
