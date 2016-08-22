using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public delegate void NotifyManagerDelegate<TKey, TData, TManager>(IProcess<TKey, TData, TManager> process);

    public interface IProcess<TKey, TData, TManager>
    {
        TManager Manager { get; set; }
        TKey GetKey();
        TData GetProcessData();
        void UpdateManager();
        IProcessManager<TKey, TData, TManager> ProcessManager { get; set; }
        TManager GetManager();
    }

    public interface IProcessManager<TKey, TData, TManager>
    {
        NotifyManagerDelegate<TKey, TData, TManager> ManagerNotifier { get; set; }
        void AddProcess(IProcess<TKey, TData, TManager> process);
        void RemoveProcess(TKey key);
        void RemoveProcess(IProcess<TKey, TData, TManager> process);
    }

    public abstract class Process<TKey, TData, TManager> : IProcess<TKey, TData, TManager>
    {
        
        NotifyManagerDelegate<TKey, TData, TManager> _managerNotifier;

        TManager _manager;
        public TManager Manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        public IProcessManager<TKey, TData, TManager> ProcessManager { get; set; }
        public abstract TKey GetKey();
        public abstract TData GetProcessData();

        public Process(NotifyManagerDelegate<TKey, TData, TManager> managerNotifier)
        {
            _managerNotifier = managerNotifier;
        }

        public TManager GetManager()
        {
            return _manager;
        }

        public void UpdateManager()
        {
            if (_managerNotifier != null)
                _managerNotifier(this);
        }
    }

    public class ProcessManager<TKey, TData, TManager> : IProcessManager<TKey, TData, TManager>
    {
        private NotifyManagerDelegate<TKey, TData, TManager> _managerNotifier;
        IDictionary<TKey, IProcess<TKey, TData, TManager>> _processes = new Dictionary<TKey, IProcess<TKey, TData, TManager>>();

        public NotifyManagerDelegate<TKey, TData, TManager> ManagerNotifier
        {
            get { return _managerNotifier; }
            set { _managerNotifier = value; }
        }

        TManager _manager;

        public ProcessManager(TManager manager)
        {
            _manager = manager;
        }

        public void AddProcess(IProcess<TKey, TData, TManager> process)
        {
            if((process == null) || (_processes.ContainsKey(process.GetKey())))
                return;

            process.ProcessManager = this;
            process.Manager = _manager;

            _processes.Add(process.GetKey(), process);
        }

        public void RemoveProcess(TKey key)
        {
            if((key == null) || (!_processes.ContainsKey(key)))
                return;

            _processes.Remove(key);
        }

        public void RemoveProcess(IProcess<TKey, TData, TManager> process)
        {
            if (process == null)
                return;

            RemoveProcess(process.GetKey());
        }
    }
}
