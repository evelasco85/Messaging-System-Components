using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public interface IProcessManager<TKey, TData, TProcessor>
    {
        void AddProcess(IProcess<TKey, TData, TProcessor> process);
        void RemoveProcess(TKey key);
        void RemoveProcess(IProcess<TKey, TData, TProcessor> process);
    }
}
