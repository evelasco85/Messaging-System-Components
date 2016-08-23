using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    

    public abstract class Process<TKey, TData, TProcessor> : IProcess<TKey, TData, TProcessor>
    {
        NotifyManagerDelegate<TKey, TData, TProcessor> _managerNotifier;

        public NotifyManagerDelegate<TKey, TData, TProcessor> ManagerNotifier
        {
            get { return _managerNotifier; }
            set { _managerNotifier = value; }
        }

        TProcessor _processor;
        public TProcessor Processor
        {
            get { return _processor; }
            set { _processor = value; }
        }

        public IProcessManager<TKey, TData, TProcessor> ProcessManager { get; set; }
        public abstract TKey GetKey();
        public abstract TData GetProcessData();
        public abstract void StartProcess(TProcessor processor);

        public void StartProcess()
        {
            StartProcess(GetProcessor());
        }

        public TProcessor GetProcessor()
        {
            if (_processor == null)
                throw new MemberAccessException("Cannot access processor. A process must be added to process manager in-order to retrieve processor instance");

            return _processor;
        }

        public void UpdateManager()
        {
            if (_managerNotifier != null)
                _managerNotifier(this);
        }
    }
}
