using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;


namespace UnityFramework.Pool
{


    public struct PoolKey : System.IEquatable<PoolKey>
    {
        private readonly System.Type typeKey;
        private readonly MonoBehaviour prefab;

        public PoolKey(System.Type type)
        {
            this.typeKey = type;    
            this.prefab = null;   
        }

        public PoolKey(MonoBehaviour prefab)
        {
            this.typeKey = prefab.GetType();
            this.prefab = prefab;
        }

        public bool Equals(PoolKey other)
        {
            return typeKey.Equals(other.typeKey);
        }

        public override int GetHashCode()
        {
            int typeHash = typeKey.GetHashCode();
            if (prefab == null)
                return typeHash;

            int prefabHash = prefab.GetInstanceID();
            return prefabHash ^ typeHash; // XOR 연산으로 해시값 조합
        }

    }

    public abstract class Pool
    {
        protected Stack<IPoolObject> objects = new Stack<IPoolObject>(4);

        public Pool(IPoolObject instance)
        {
        }

        public IPoolObject GetObject()
        {
            IPoolObject poolObject = null;
            bool isValid = true;
            while (isValid)
            {
                if (objects.Count == 0)
                    poolObject = CreateObject();
                else
                    poolObject = objects.Pop();
                // 혹시라도 생성되있는애가 풀에 들어와 있을경우
                isValid = poolObject.IsValid();
            }
            return objects.Pop();
        }

        protected abstract IPoolObject CreateObject();

        public void SetObject(IPoolObject classObject)
        {
            objects.Push(classObject);
        }
    }

    public class ClassPool<T> : Pool where T : class, IPoolObject, new()
    {
        public ClassPool(IPoolObject instance) : base(instance)
        {
        }

        protected override IPoolObject CreateObject()
        {
            return new T();
        }
    }

    public class MonoPool<T> : Pool where T : MonoBehaviour, IPoolObject
    {
        T monoInstance;
        public MonoPool(IPoolObject instance) : base(instance)
        {
            monoInstance = instance as T;
            if (monoInstance == null)
                throw new System.Exception($"{((MonoBehaviour)instance).name} Not {typeof(T).Name} is different type");
        }

        protected override IPoolObject CreateObject()
        {
            return GameObject.Instantiate(monoInstance);
        }
    }

}