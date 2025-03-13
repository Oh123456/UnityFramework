using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFramework.Pool.Manager
{
    public class MonoPoolManager
    {
        Dictionary<PoolKey, Pool> monoPool = new Dictionary<PoolKey, Pool>();

        public T GetObject<T>(IPoolObject prefab, bool isAutoActivate = true) where T : MonoBehaviour, IPoolObject, new()
        {
            Pool pool = GetPool<T>(prefab);
            IPoolObject poolObject = pool.GetObject();
            if (isAutoActivate)
                poolObject.Activate();
            return poolObject as T;
        }

        public void SetObject<T>(IPoolObject poolObject, bool isAutoDeactivate = true) where T : MonoBehaviour, IPoolObject
        {
            if (!FIndPool((MonoBehaviour)poolObject, out Pool pool))
            {
                Debug.Log("юс╫ц");
                return;
            }
            if (isAutoDeactivate)
                poolObject.Deactivate();
            pool.SetObject(poolObject);
        }

        Pool GetPool<T>(IPoolObject poolObject) where T : MonoBehaviour, IPoolObject, new()
        {
            PoolKey poolKey = new PoolKey((MonoBehaviour)poolObject);

            if (!monoPool.TryGetValue(poolKey, out Pool pool))
            {
                pool = new MonoPool<T>(poolObject);
                monoPool.Add(poolKey, pool);
            }

            return pool;
        }

        bool FIndPool(MonoBehaviour monoBehaviour , out Pool pool)
        {
            PoolKey poolKey = new PoolKey(monoBehaviour);

            return monoPool.TryGetValue(poolKey, out pool);
        }
    }
}