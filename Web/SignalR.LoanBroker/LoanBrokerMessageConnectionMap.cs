using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.SignalR.LoanBroker
{
    public class LoanBrokerMessageConnectionMap
    {
        public string ConnectionId { get; set; }
        public string MessageId { get; set; }
    }

    public class ConnectionMapper
    {
        readonly IList<LoanBrokerMessageConnectionMap> _connections = new List<LoanBrokerMessageConnectionMap>();

        public void Add(string connectionId, string messageId)
        {
            lock (_connections)
            {
                _connections.Add(new LoanBrokerMessageConnectionMap
                {
                    ConnectionId = connectionId,
                    MessageId = messageId
                });
            }
        }

        public IEnumerable<string> GetConnections(string messageId)
        {
            if (_connections.Any(connection => connection.MessageId == messageId))
                return _connections
                    .Where(connection => connection.MessageId == messageId)
                    .Select(connection => connection.ConnectionId);

            return Enumerable.Empty<string>();
        }

        public void RemoveByConnectionId(string connectionId)
        {
            lock (_connections)
            {
                ((List<LoanBrokerMessageConnectionMap>) _connections)
                    .RemoveAll(connection => connection.ConnectionId == connectionId);
            }
        }

        public void RemoveByMessageId(string messageId)
        {
            lock (_connections)
            {
                ((List<LoanBrokerMessageConnectionMap>)_connections)
                    .RemoveAll(connection => connection.MessageId == messageId);
            }
        }
    }
}