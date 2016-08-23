using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public delegate void NotifyManagerDelegate<TKey, TData, TProcessor>(IProcess<TKey, TData, TProcessor> process);

    public interface IProcess<TKey, TData, TProcessor>
    {
        TProcessor Processor { get; set; }
        TKey GetKey();
        TData GetProcessData();
        void UpdateManager();
        IProcessManager<TKey, TData, TProcessor> ProcessManager { get; set; }
        NotifyManagerDelegate<TKey, TData, TProcessor> ManagerNotifier { get; set; }
        TProcessor GetProcessor();
        void StartProcess();
        void StartProcess(TProcessor processor);
    }
}
