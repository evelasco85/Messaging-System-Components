using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Routing
{
    public abstract class Aggregate<TKey, TValue, TAggregatedValue> : IAggregate<TKey, TValue, TAggregatedValue>
    {
        private TKey _Key;

        public TKey Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        IList<TValue> _aggregatedValues = new List<TValue>();
        private Func<IList<TValue>, bool> _aggregateCompletionCondition;
        private OnNotifyAggregationCompletion<TAggregatedValue> _onAggregationCompletion;

        public Aggregate(TKey key,
            Func<IList<TValue>, bool> aggregateCompletionCondition,
            OnNotifyAggregationCompletion<TAggregatedValue> onAggregationCompletion
            )
        {
            if (aggregateCompletionCondition == null)
                throw new ArgumentNullException("'aggregateCompletionCondition' parameter is required");

            _Key = key;
            _aggregateCompletionCondition = aggregateCompletionCondition;
            _onAggregationCompletion += onAggregationCompletion;
        }

        public void AggregateValue(TValue value)
        {
            PreAggregation(ref value);
            _aggregatedValues.Add(value);
            PostAggregation(value);
        }

        public bool IsComplete()
        {
            return _aggregateCompletionCondition(_aggregatedValues);
        }

        public virtual void PreAggregation(ref TValue value)
        {
        }

        public virtual void PostAggregation(TValue value)
        {
        }

        public abstract TAggregatedValue PerformAggregation(IList<TValue> aggregatedValues);

        public TAggregatedValue GetAggregatedValue()
        {
            return PerformAggregation(_aggregatedValues);
        }

        public void NotifyAggregationCompletion()
        {
            if (_onAggregationCompletion != null)
                _onAggregationCompletion(GetAggregatedValue());
        }
    }

    public class Aggregator<TKey, TValue, TAggregatedValue, TAggregate> : IAggregator<TKey, TValue, TAggregatedValue, TAggregate>
        where TAggregate : IAggregate<TKey, TValue, TAggregatedValue>
    {
        Dictionary<TKey, IAggregate<TKey, TValue, TAggregatedValue>> _aggregates = new Dictionary<TKey, IAggregate<TKey, TValue, TAggregatedValue>>();

        public int GetAggregateCount()
        {
            return _aggregates.Count;
        }

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

        public void RemoveAggregate(IAggregate<TKey, TValue, TAggregatedValue> aggregate)
        {
            if (aggregate == null)
                return;

            RemoveAggregate(aggregate.Key);
        }
    }
}
