using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public delegate void NotifyManagerDelegate<TKey, TData>(IProcess<TKey, TData> process);

    public interface IProcess<TKey, TData>
    {
        TKey GetKey();
        TData GetProcessData();
        void UpdateManager();
        IProcessManager<TKey, TData> ProcessManager { get; set; }
    }

    public interface IProcessManager<TKey, TData>
    {
        NotifyManagerDelegate<TKey, TData> ManagerNotifier { get; set; }
        void AddProcess(IProcess<TKey, TData> process);
        void RemoveProcess(TKey key);
        void RemoveProcess(IProcess<TKey, TData> process);
    }

    public abstract class Process<TKey, TData> : IProcess<TKey, TData>
    {
        NotifyManagerDelegate<TKey, TData> _managerNotifier;

        public IProcessManager<TKey, TData> ProcessManager { get; set; }

        public abstract TKey GetKey();
        public abstract TData GetProcessData();

        public Process(NotifyManagerDelegate<TKey, TData> managerNotifier)
        {
            _managerNotifier = managerNotifier;
        }

        public void UpdateManager()
        {
            if (_managerNotifier != null)
                _managerNotifier(this);
        }
    }

    public class ProcessManager<TKey, TData> : IProcessManager<TKey, TData>
    {
        private NotifyManagerDelegate<TKey, TData> _managerNotifier;
        IDictionary<TKey, IProcess<TKey, TData>> _processes = new Dictionary<TKey, IProcess<TKey, TData>>();

        public NotifyManagerDelegate<TKey, TData> ManagerNotifier
        {
            get { return _managerNotifier; }
            set { _managerNotifier = value; }
        }

        public void AddProcess(IProcess<TKey, TData> process)
        {
            if((process == null) || (_processes.ContainsKey(process.GetKey())))
                return;

            process.ProcessManager = this;

            _processes.Add(process.GetKey(), process);
        }

        public void RemoveProcess(TKey key)
        {
            if((key == null) || (!_processes.ContainsKey(key)))
                return;

            _processes.Remove(key);
        }

        public void RemoveProcess(IProcess<TKey, TData> process)
        {
            if (process == null)
                return;

            RemoveProcess(process.GetKey());
        }
    }
}
