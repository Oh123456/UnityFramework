using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityFramework.Pool;

namespace UnityFramework.PoolObject
{
    public interface IPoolObject
    {
        public bool IsValid();
        public void Activate();
        public void Deactivate();
    }

    public interface IMonoPoolObject : IPoolObject
    {
        public int KeyCode { get; set; }
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

    public abstract class MonoPoolObject : MonoBehaviour, IMonoPoolObject
    {
        bool isActive = false;
        int keyCode = 0;
        public int KeyCode { get => keyCode; set => keyCode = value; }

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

        public T this[int index]
        {
            get { return array[index]; }
            set { array[index] = value; }
        }

        public T[] Array => array;

        public int MaxLength => array.Length;

        public ArrayPoolObject(int size)
        {
            array = ArrayPool<T>.Shared.Rent(size);
        }

        public void SetDatas(T[] values)
        {
            int lenght = values.Length;
            for (int i = 0; i < lenght; i++)
            {
                array[i] = values[i];
            }
        }

        public void Dispose()
        {
            Dispose(true);
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
