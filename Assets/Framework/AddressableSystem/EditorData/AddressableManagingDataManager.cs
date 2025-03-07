using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using static UnityFramework.Addressable.Editor.AddressableManagingDataManager;


#if UNITY_EDITOR
namespace UnityFramework.Addressable.Editor
{
    public static class AddressableManagingDataManager
    {
        public enum LoadType
        {
            UnsafeLoad = 0,
            SafeLoad,
            Max,
        }

        public const int UNSAFE_LOAD_STACK_COUNT = 3;
        public const int SAFE_LOAD_STACK_COUNT = 4;
        public static Dictionary<string, AddressableManagingData> addressableManagingDatas = new Dictionary<string, AddressableManagingData>();
        private static bool? isTracking;

        public static bool IsTracking
        {
            private get
            {
                if (isTracking == null)
                    isTracking = EditorPrefs.GetBool("_isTracking", true);
                return isTracking.Value;
            }

            set
            {
                isTracking = value;
            }
        }

        public static void TrackEditorLoad<T>(AsyncOperationHandle<T> handle, LoadType loadType, object key)
        {
            System.Diagnostics.StackTrace stackTrace = null;


            if (IsTracking)
            {
                stackTrace = new System.Diagnostics.StackTrace(true);
            }

            handle.Completed += (AsyncOperationHandle<T> handle) =>
            {
                Object loadedAsset = handle.Result as Object;
                if (loadedAsset == null)
                {
                    AddressableManager.AddressableLog($"{handle.DebugName} is Not Object Type", Color.yellow);
                    return;
                }

                string assetPath = AssetDatabase.GetAssetPath(loadedAsset);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                    if (!addressableManagingDatas.TryGetValue(assetGUID, out var data))
                    {
                        data = new AddressableManagingData();
                        addressableManagingDatas.Add(assetGUID, data);
                    }
                    data.loadCount++;
                    object accessKey = key is IKeyEvaluator keyEvaluator ? keyEvaluator.RuntimeKey : key;
                    data.accessKeys.Add(accessKey);
                    data.name = handle.DebugName;
                    if (stackTrace != null)
                    {
                        var traces = data.GetAddressableManagingDataStackTrace(loadType);

                        int index = loadType == LoadType.UnsafeLoad ? UNSAFE_LOAD_STACK_COUNT : SAFE_LOAD_STACK_COUNT;
                        System.Diagnostics.StackFrame frame = stackTrace.GetFrame(index - 1);
                        string info = $"{frame.GetMethod().DeclaringType}.{frame.GetMethod().Name} ({frame.GetFileName()}:{frame.GetFileLineNumber()})";
                        if (!traces.TryGetValue(info, out var trace))
                        {
                            trace = new AddressableManagingData.AddressableManagingDataStackTrace();
                            traces.Add(info, trace);
                        }
                        trace.stackTrace = info;
                        trace.count++;
                    }

                    AddressableManager.AddressableLog($"Addressable 에셋의 GUID: {assetGUID}", Color.yellow);
                }
                else
                {
                    AddressableManager.AddressableLog(" 이 Addressable 에셋은 프로젝트 내부에 없음.", Color.red);
                }
            };
        }


        public static void TrackRelease<T>(AsyncOperationHandle<T> handle)
        {
            Object loadedAsset = handle.Result as Object;

            string assetPath = AssetDatabase.GetAssetPath(loadedAsset);
            if (!string.IsNullOrEmpty(assetPath))
            {
                string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                if (!addressableManagingDatas.TryGetValue(assetGUID, out var data))
                {
                    data = new AddressableManagingData();
                    addressableManagingDatas.Add(assetGUID, data);
                }
                data.loadCount--;

                AddressableManager.AddressableLog($"Addressable 에셋의 GUID: {assetGUID}", Color.yellow);
            }
        }
    }

    public class AddressableManagingData
    {
        public class AddressableManagingDataStackTrace : System.IEquatable<AddressableManagingDataStackTrace>
        {
            public string stackTrace = string.Empty;
            public int count = 0;

            public bool Equals(AddressableManagingDataStackTrace other)
            {
                return stackTrace == other.stackTrace;
            }

            public override int GetHashCode()
            {
                return stackTrace.GetHashCode();
            }

        }

        public HashSet<object> accessKeys = new HashSet<object>();
        public Dictionary<LoadType, Dictionary<string, AddressableManagingDataStackTrace>> stackTraces = new Dictionary<LoadType, Dictionary<string, AddressableManagingDataStackTrace>>((int)LoadType.Max);
        public string name;
        public int loadCount = 0;
        /// <summary>
        /// 에디터용
        /// </summary>
        public bool foldout = false;

        public Dictionary<string, AddressableManagingDataStackTrace> GetAddressableManagingDataStackTrace(LoadType loadType)
        {
            switch (loadType)
            {
                case LoadType.UnsafeLoad:
                case LoadType.SafeLoad:
                    if (!stackTraces.TryGetValue(loadType, out var trces))
                    {
                        trces = new Dictionary<string, AddressableManagingDataStackTrace>();
                        stackTraces.Add(loadType, trces);
                    }
                    return trces;
                case LoadType.Max:
                default:
                    return null;
            }
        }

    }
}

#endif