using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Routing
{
    public delegate void OnNotifyAggregationCompletion<TAggregatedValue>(TAggregatedValue aggregatedValue);

    public interface IAggregate<TKey, TValue, TAggregatedValue>
    {
        TKey Key { get; set; }
        void AggregateValue(TValue value);
        bool IsComplete();
        void PreAggregation(ref TValue value);
        void PostAggregation(TValue value);
        TAggregatedValue PerformAggregation(IList<TValue> aggregatedValues);
        TAggregatedValue GetAggregatedValue();
        void NotifyAggregationCompletion();
    }
}
