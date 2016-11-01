using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Shared.Models
{
    public enum ClientCommandStatus
    {
        Inactive = 0,
        InvalidRegistration,
        SetupClientParameters,
        Standby,
        Start,
        Stop,
        ServerAnnouncement
    }

    public enum ServerRequestType
    {
        None = 0,
        Register
    }

    public class ServerMessage
    {
        //public QueueTypeEnum QueueType { get; set; }
        public Guid ClientId { get; set; }
        public ClientCommandStatus ClientStatus { get; set; }
        public IDictionary<string, object> ClientParameters { get; set; }
        public string Message { get; set; }
    }

    public class ServerRequest
    {
        public ServerRequestType RequestType { get; set; }
        public Guid ClientId { get; set; }
        public List<string> ParameterList { get; set; }
    }
}
