using UnityEngine;
using UnityFramework.Pool.Manager;

namespace UnityFramework.Pool
{


    public static class PoolManager
    {
        #region Class

        static System.Lazy<ClassPoolManager> classPoolManager = new System.Lazy<ClassPoolManager>(() => new ClassPoolManager());


        public static T GetClassObject<T>(bool isAutoActivate = true) where T : class, IPoolObject, new()
        {
            return classPoolManager.Value.GetObject<T>(isAutoActivate: isAutoActivate);
        }

        public static void SetClassObject(IPoolObject poolObject, bool isAutoDeactivate = true)
        {
            classPoolManager.Value.SetObject(poolObject, isAutoDeactivate: isAutoDeactivate);
        }
        #endregion

        #region Mono
        static System.Lazy<MonoPoolManager> monoPoolManager = new System.Lazy<MonoPoolManager>(() => new MonoPoolManager());

        public static T GetMonoObject<T>(IPoolObject prefab, bool isAutoActivate = true) where T : MonoBehaviour, IPoolObject, new()
        {
            return monoPoolManager.Value.GetObject<T>(prefab, isAutoActivate: isAutoActivate);
        }

        public static void SetMonoObject<T>(IPoolObject poolObject, bool isAutoDeactivate = true) where T : MonoBehaviour, IPoolObject
        {
            monoPoolManager.Value.SetObject<T>(poolObject, isAutoDeactivate: isAutoDeactivate);
        }

        #endregion


        public static ArrayPoolObject<T> GetArray<T>(int size, bool clearArray = false)
        {
            return new ArrayPoolObject<T>(size, clearArray: clearArray);
        }
    }

}