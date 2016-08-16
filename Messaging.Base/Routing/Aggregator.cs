using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Routing
{
    public class Aggregate<TKey, TValue, TAggregatedValue>
    {
        private TKey _Key;

        public TKey Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        IList<TValue> _aggregatedValues = new List<TValue>();
        private Func<IList<TValue>, bool> _aggregateCompletionCondition;

        public Aggregate(TKey key, Func<IList<TValue>, bool> aggregateCompletionCondition)
        {
            if(aggregateCompletionCondition == null)
                throw new ArgumentNullException("'aggregateCompletionCondition' parameter is required");

            _Key = key;
            _aggregateCompletionCondition = aggregateCompletionCondition;
        }

        public void AddValue(TValue value)
        {
            _aggregatedValues.Add(value);
        }

        public bool IsComplete()
        {
            return _aggregateCompletionCondition(_aggregatedValues);
        }

        //public TAggregatedValue GetAggregatedValue()
        //{
        //    TAggregatedValue value = default(TAggregatedValue);

        //    return value;
        //}
    }

    public class Aggregator
    {
    }
}
