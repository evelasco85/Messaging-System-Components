using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
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
