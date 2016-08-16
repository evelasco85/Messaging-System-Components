using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Routing
{
    public class Aggregator<TKey, TValue, TAggregatedValue, TAggregate> : IAggregator<TKey, TValue, TAggregatedValue, TAggregate>
        where TAggregate : IAggregate<TKey, TValue, TAggregatedValue>
    {
        Dictionary<TKey, IAggregate<TKey, TValue, TAggregatedValue>> _aggregates = new Dictionary<TKey, IAggregate<TKey, TValue, TAggregatedValue>>();

        public void AddAggregate(TKey key, TAggregate aggregate)
        {
            _aggregates.Add(key, aggregate);
        }

        public bool Contains(TKey key)
        {
            return _aggregates.ContainsKey(key);
        }

        public TAggregate GetAggregate(TKey key)
        {
            if (Contains(key))
                return (TAggregate)_aggregates[key];

            return default(TAggregate);
        }

        public void RemoveAggregate(TKey key)
        {
            if (Contains(key))
                _aggregates.Remove(key);
        }
    }
}
