using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Orchestration.Models
{
    public interface IVersionInfo<TVersion>
    {
        TVersion Version { get; }
        QueueTypeEnum QueueType { get; }
        Guid ID { get; }
    }

    public class VersionInfo<TVersion> : IVersionInfo<TVersion>
    {
        private TVersion _version;
        private QueueTypeEnum _queueType;
        private Guid _id;

        public TVersion Version
        {
            get { return _version; }
        }

        public QueueTypeEnum QueueType
        {
            get { return _queueType; }
        }

        public Guid ID
        {
            get
            {
                return _id;
            }
        }

        public VersionInfo(QueueTypeEnum queueType, Guid id, TVersion version)
        {
            _id = id;
            _queueType = queueType;
            _version = version;
        }
    }
}
