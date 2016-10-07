using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base.Constructions
{
    public class CorrelationEntity<TKey, TEntity> : ICorrelationEntity<TKey, TEntity>
    {
        TKey _key;
        private TEntity _entity;

        public TKey Key { get { return _key; } }
        public TEntity Entity { get { return _entity; } }

        public CorrelationEntity(TKey key, TEntity entity)
        {
            _key = key;
            _entity = entity;
        }
    }

    public class CorrelationManager<TKey, TEntity> : ICorrelationManager<TKey, TEntity>
    {
        IDictionary<TKey, ICorrelationEntity<TKey, TEntity>> _collection = new Dictionary<TKey, ICorrelationEntity<TKey, TEntity>>();

        public void AddEntity(TKey key, TEntity entity)
        {
            AddEntity(new CorrelationEntity<TKey, TEntity>(key, entity));
        }

        public void AddEntity(ICorrelationEntity<TKey, TEntity> entity)
        {
            if((entity != null) && ((!_collection.ContainsKey(entity.Key))))
                _collection.Add(entity.Key, entity);
        }

        public bool EntityExists(TKey key)
        {
            return _collection.ContainsKey(key);
        }

        public TEntity GetEntity(TKey key)
        {
            TEntity entity = default(TEntity);

            if (EntityExists(key))
                entity = _collection[key].Entity;

            return entity;
        }

        public void RemoveEntity(TKey key)
        {
            if (EntityExists(key))
                _collection.Remove(key);
        }

        public void RemoveEntity( ICorrelationEntity<TKey, TEntity> entity)
        {
            if (entity != null)
                RemoveEntity(entity.Key);
        }

        public int Count
        {
            get { return _collection.Count; }
        }
    }
}
