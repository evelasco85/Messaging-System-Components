using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Base.Routing
{
    public class Aggregate<TKey, TValue, TAggregatedValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        IList<TValue> _valueList = new List<TValue>();
        private Func<IList<TValue>, bool> _aggregateCompletionCondition;

        public Aggregate(TKey key, Func<IList<TValue>, bool> aggregateCompletionCondition)
        {
            if(aggregateCompletionCondition == null)
                throw new ArgumentNullException("'aggregateCompletionCondition' parameter is required");

            _aggregateCompletionCondition = aggregateCompletionCondition;
        }

        public void AddValue(TValue value)
        {
            _valueList.Add(value);
        }

        public bool IsComplete()
        {
            return _aggregateCompletionCondition(_valueList);
        }

        public TAggregatedValue GetAggregatedValue()
        {
            TAggregatedValue value = default(TAggregatedValue);

            return value;
        }
    }

    public class Aggregator
    {
    }
}
