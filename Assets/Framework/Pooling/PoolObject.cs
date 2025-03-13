using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
namespace UnityFramework.Pool
{
    public interface IPoolObject
    {
        public bool IsValid();
        public void Activate();
        public void Deactivate();
    }


    public abstract class ClassPoolObject : IPoolObject, System.IDisposable
    {
        bool isActive = false;

        public virtual void Activate()
        {
            isActive = true;
        }

        public virtual void Deactivate()
        {
            isActive = false;
        }

        public void Dispose()
        {
            PoolManager.SetClassObject(this, isAutoDeactivate: true);
        }

        public bool IsValid()
        {
            return isActive;
        }
    }

    public abstract class MonoPoolObject : MonoBehaviour, IPoolObject
    {
        bool isActive = false;

        public virtual void Activate()
        {
            isActive = true;
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            isActive = false;
            gameObject.SetActive(false);
        }

        public bool IsValid()
        {
            return isActive;
        }
    }

    public struct ArrayPoolObject<T> : System.IDisposable
    {
        T[] array;
        bool clearArray;

        public T this[int index]
        {
            get { return array[index]; }
            set { array[index] = value; }
        }

        public int MaxLength => array.Length;

        public ArrayPoolObject(int size, bool clearArray)
        {
            array = ArrayPool<T>.Shared.Rent(size);
            this.clearArray = clearArray;
        }

        public void Dispose()
        {
            Dispose(clearArray);
        }

        public void Dispose(bool clearArray)
        {
            ArrayPool<T>.Shared.Return(array, clearArray: clearArray);
            array = null;
        }

        public bool IsValid()
        {
            return array != null;
        }
    }

}
