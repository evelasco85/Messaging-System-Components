using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public delegate void NotifyManagerDelegate<TKey, TProcessData>(IProcess<TKey, TProcessData> process);

    public interface IProcess<TKey, TProcessData>
    {
        TKey GetKey();
        TProcessData GetProcessData();
        void PerformNotifyManager();
        IProcessManager<TKey, TProcessData> ProcessManager { get; set; }
        NotifyManagerDelegate<TKey, TProcessData> NotifyManager { get; set; }
    }

    public interface IProcessManager<TKey, TProcessData>
    {
        NotifyManagerDelegate<TKey, TProcessData> ManagerNotifier { get; set; }
        void AddProcess(IProcess<TKey, TProcessData> process);
        void RemoveProcess(TKey key);
        void RemoveProcess(IProcess<TKey, TProcessData> process);
    }

    public abstract class Process<TKey, TProcessData> : IProcess<TKey, TProcessData>
    {
        public IProcessManager<TKey, TProcessData> ProcessManager { get; set; }
        public NotifyManagerDelegate<TKey, TProcessData> NotifyManager { get; set; }

        public abstract TKey GetKey();
        public abstract TProcessData GetProcessData();

        public void PerformNotifyManager()
        {
            if (NotifyManager != null)
                NotifyManager(this);
        }
    }

    public class ProcessManager<TKey, TProcessData> : IProcessManager<TKey, TProcessData>
    {
        private NotifyManagerDelegate<TKey, TProcessData> _managerNotifier;

        IDictionary<TKey, IProcess<TKey, TProcessData>> _processes = new Dictionary<TKey, IProcess<TKey, TProcessData>>();

        public NotifyManagerDelegate<TKey, TProcessData> ManagerNotifier
        {
            get { return _managerNotifier; }
            set { _managerNotifier = value; }
        }

        public void AddProcess(IProcess<TKey, TProcessData> process)
        {
            if((process == null) || (_processes.ContainsKey(process.GetKey())))
                return;

            process.ProcessManager = this;
            process.NotifyManager = _managerNotifier;

            _processes.Add(process.GetKey(), process);
        }

        public void RemoveProcess(TKey key)
        {
            if((key == null) || (!_processes.ContainsKey(key)))
                return;

            _processes.Remove(key);
        }

        public void RemoveProcess(IProcess<TKey, TProcessData> process)
        {
            if (process == null)
                return;

            RemoveProcess(process.GetKey());
        }
    }
}
