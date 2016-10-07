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

    public class ProcessManager<TKey, TData, TProcessor> : IProcessManager<TKey, TData, TProcessor>
    {
        private NotifyManagerDelegate<TKey, TData, TProcessor> _managerNotifier;
        IDictionary<TKey, IProcess<TKey, TData, TProcessor>> _processes = new Dictionary<TKey, IProcess<TKey, TData, TProcessor>>();

        TProcessor _processor;

        public ProcessManager(TProcessor processor, NotifyManagerDelegate<TKey, TData, TProcessor> managerNotifier)
        {
            if (processor == null)
                throw new ArgumentNullException("'processor' parameter requires a value");

            _processor = processor;
            _managerNotifier = managerNotifier;
        }

        public void AddProcess(IProcess<TKey, TData, TProcessor> process)
        {
            if((process == null) || (_processes.ContainsKey(process.GetKey())))
                return;

            process.ProcessManager = this;
            process.Processor = _processor;
            process.ManagerNotifier = _managerNotifier;

            _processes.Add(process.GetKey(), process);
        }

        public void RemoveProcess(TKey key)
        {
            if((key == null) || (!_processes.ContainsKey(key)))
                return;

            _processes.Remove(key);
        }

        public void RemoveProcess(IProcess<TKey, TData, TProcessor> process)
        {
            if (process == null)
                return;

            RemoveProcess(process.GetKey());
        }
    }
}
