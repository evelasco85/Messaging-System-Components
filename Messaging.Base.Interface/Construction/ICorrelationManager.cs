using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Constructions
{
    public interface ICorrelationManager<TKey, TEntity>
    {
        int Count { get; }
        void AddEntity(TKey key, TEntity entity);
        void AddEntity(ICorrelationEntity<TKey, TEntity> entity);
        bool EntityExists(TKey key);
        TEntity GetEntity(TKey key);
        void RemoveEntity(TKey key);
        void RemoveEntity(ICorrelationEntity<TKey, TEntity> entity);
    }

    public interface ICorrelationEntity<TKey, TEntity>
    {
        TKey Key { get; }
        TEntity Entity { get; }
    }
}
