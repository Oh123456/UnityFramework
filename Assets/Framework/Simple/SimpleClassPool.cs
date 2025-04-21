using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityFramework.PoolObject;

namespace UnityFramework
{
    public class SimpleClassPool<T> where T : class, IPoolObject , new()
    {
        private Stack<T> classPool = new Stack<T>(4);

        public T Get(bool isAutoActive = true)
        {
            T obj = classPool.Count == 0 ? new T() : classPool.Pop();
            if (isAutoActive)
                obj.Activate(); 
            return obj;
        }

        public void Set(IPoolObject tClass , bool isAutoDeactive = true)
        {
            tClass.Deactivate();
            classPool.Push((T)tClass);
        }

        public void Clear()
        {
            while (classPool.Count != 0)
                classPool.Pop();
        }
    }

}