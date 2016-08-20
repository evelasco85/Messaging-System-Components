using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public interface IAggregator<TKey, TValue, TAggregatedValue, TAggregate>
        where TAggregate : IAggregate<TKey, TValue, TAggregatedValue>
    {
        void AddAggregate(TKey key, TAggregate aggregate);
        bool Contains(TKey key);
        TAggregate GetAggregate(TKey key);
        void RemoveAggregate(TKey key);
        void RemoveAggregate(IAggregate<TKey, TValue, TAggregatedValue> aggregate);
    }
}
