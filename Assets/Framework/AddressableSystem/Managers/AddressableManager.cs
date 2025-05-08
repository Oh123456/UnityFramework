using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;




#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
using TaksDownLoadData = System.Threading.Tasks.Task<UnityFramework.Addressable.AddressableDownLoadData>;
using TaksResourceLocator = System.Threading.Tasks.Task<System.Collections.Generic.List<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator>>;
using TaksLables = System.Threading.Tasks.Task<System.Collections.Generic.List<string>>;
using TaskSize = System.Threading.Tasks.Task<long>;
#else
using Cysharp.Threading.Tasks;
//List<IResourceLocator>
using TaksDownLoadData = Cysharp.Threading.Tasks.UniTask<UnityFramework.Addressable.AddressableDownLoadData>;
using TaksResourceLocator = Cysharp.Threading.Tasks.UniTask<System.Collections.Generic.List<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator>>;
using TaksLables = Cysharp.Threading.Tasks.UniTask<System.Collections.Generic.List<string>>;
using TaskSize = Cysharp.Threading.Tasks.UniTask<long>;
#endif

namespace UnityFramework.Addressable
{
    public enum ByteUnit : long
    {
        B = 1,
        KB = 1024,
        MB = 1024 * 1024,
        GB = 1024 * 1024 * 1024,
    }

    public enum AddressableDownLoadState
    {
        CheckForCatalogUpdates,
        UpdateCatalogs,

    }

    public struct AddressableDownLoadError
    {
        public AddressableDownLoadState state;
        public string log;
    }

    public struct AddressableDownLoadData
    {
        public List<string> labels;
        public long size;
    }

    public struct AddressableDownLoadUtility
    {
        public static ByteUnit GetByteUnit(long downLoadSize)
        {
            if (downLoadSize < (long)ByteUnit.KB)
                return ByteUnit.B;
            if (downLoadSize < (long)ByteUnit.MB)
                return ByteUnit.KB;
            if (downLoadSize < (long)ByteUnit.GB)
                return ByteUnit.MB;

            return ByteUnit.GB;
        }

        public static float ToUnit(long downLoadSize, ByteUnit byteUnit)
        {
            return (float)downLoadSize / (float)byteUnit;
        }
    }

    public partial class AddressableManager : Singleton.LazySingleton<AddressableManager>
    {
        public const string BUILD_LABELS_PATH = "Assets/Resources";

        public event Action OnAllCompletedLoad;
        public event Action<AsyncOperationHandle> OnDownload;
        public event Action<AddressableDownLoadError> OnDownLoadError;
        private event Action OnCompletedLoad;

        private AddressableBuildLabels addressableBuildLabels;

        private AddressableBuildLabels AddressableBuildLabels
        {
            get
            {
                if (addressableBuildLabels == null)
                    addressableBuildLabels = Resources.Load<AddressableBuildLabels>(AddressableBuildLabels.NAME);

                if (addressableBuildLabels == null)
                {
                    AddressableLog($"AddressableBuildLabels Not Found");
                    return null;
                }

                return addressableBuildLabels;
            }
        }

        public AddressableManager()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += (playModeStateChange) =>
            {
                switch (playModeStateChange)
                {
                    case UnityEditor.PlayModeStateChange.EnteredEditMode:
                        break;
                    case UnityEditor.PlayModeStateChange.ExitingEditMode:
                        break;
                    case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                        break;
                    case UnityEditor.PlayModeStateChange.ExitingPlayMode:
                        if (this.addressableDataManager.IsValueCreated)
                            this.addressableDataManager.Value.Release();
                        break;
                }
                if (playModeStateChange == UnityEditor.PlayModeStateChange.EnteredPlayMode)
                {

                }
                Debug.Log($"PlayModeStateChange : {playModeStateChange}");
            };
#endif

        }

        public async TaksDownLoadData CheckDownLoadBundle(List<string> customLabels = null)
        {
            AddressableLog("DownLoadCheck");
            await Addressables.InitializeAsync();
            List<string> catalog = await CheckForCatalogUpdates();
            List<string> labels = null;

            if (catalog == null || catalog.Count == 0)
            {
                AddressableLog("Not Update Data");
                return new AddressableDownLoadData() { labels = null, size = 0 };
            }

            List<IResourceLocator> resourceLocators = await UpdateCatalogs(catalog);

            // 로드 라벨과 갱신된 라벨 교차 검증
            HashSet<object> loadLabels = CheckLabels(customLabels);

            int count = resourceLocators.Count;
            for (int i = 0; i < count; i++)
            {
                var keys = resourceLocators[i].Keys;
                loadLabels.IntersectWith(keys);
            }

            long size = await GetDownloadSizeAsync(loadLabels);

            if (size > 0)
            {
                AddressableLog($"DonwLoadSize : {size}");

                labels = new List<string>(loadLabels.Count);

                foreach (object key in loadLabels)
                {
                    if (key is string label)
                        labels.Add(label);
                }

            }

            return new AddressableDownLoadData()
            {
                size = size,
                labels = labels,
            };

        }

        public void DownLoadBundle()
        {
            var addressableBuildLabels = AddressableBuildLabels;
            if (addressableBuildLabels == null)
                return;

            DownLoadBundle(addressableBuildLabels.Labels);

        }
        public async void DownLoadBundle(List<string> downLoadLabels)
        {
            AddressableLog("Addressables Start");
            //var handle = Addressables.DownloadDependenciesAsync(labels, Addressables.MergeMode.Union);
            var handle = Addressables.DownloadDependenciesAsync(downLoadLabels, autoReleaseHandle: false);
            this.OnDownload?.Invoke(handle);

#if USE_ADDRESSABLE_TASK
            await handle.Task;
#else
            await handle.ToUniTask();
#endif

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AddressableLog("Download Completed", Color.green);
                CompletedLoad();
                return;
            }
            AddressableLog("Download Failed", Color.red);
            Addressables.Release(handle);
            CompleteAll();
        }


        private async TaksLables CheckForCatalogUpdates()
        {
            var handle = Addressables.CheckForCatalogUpdates(false);

#if USE_ADDRESSABLE_TASK
            await handle.Task;
#else
            await handle.ToUniTask();
#endif

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                ExecuteError($"Error CheckForCatalogUpdates {handle.Status}");
                return null;
            }

            var catalog = handle.Result;
            Addressables.Release(handle);
            return catalog;
        }

        private async TaksResourceLocator UpdateCatalogs(List<string> catalog)
        {
            var handle = Addressables.UpdateCatalogs(catalog, false);

#if USE_ADDRESSABLE_TASK
            await handle.Task; 
#else
            await handle.ToUniTask();
#endif

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                ExecuteError($"Error UpdateCatalogs {handle.Status}");
                return null;
            }

            List<IResourceLocator> resourceLocators = handle.Result;
            Addressables.Release(handle);
            return resourceLocators;
        }

        private async TaskSize GetDownloadSizeAsync(HashSet<object> loadLabels)
        {

            var handle = Addressables.GetDownloadSizeAsync(loadLabels);

#if USE_ADDRESSABLE_TASK
            await handle.Task; 
#else
            await handle.ToUniTask();
#endif

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                ExecuteError($"Error GetDownloadSizeAsync {handle.Status}");
                return 0;
            }

            long size = handle.Result;
            Addressables.Release(handle);

            return size;
        }

        #region Legacy

        //        /// <summary>
        //        /// A function in Addressables that downloads remote asset bundles, storing them in the local cache via the network, allowing them to be loaded later. 
        //        /// </summary>
        //        /// <param name="customLabels">If the list of labels to download is set to null, all currently used labels are automatically detected, and the corresponding assets are downloaded. This allows necessary resources to be loaded without explicitly specifying labels.</param>
        //        [System.Obsolete("Use DownLoadBundle")]
        //        public async void DownLoad(List<string> customLabels = null)
        //        {
        //            List<string> labelNames;
        //            AddressableLog("Addressables Start");
        //            if (customLabels == null)
        //            {
        //                AddressableBuildLabels labels = Resources.Load<AddressableBuildLabels>(AddressableBuildLabels.NAME);

        //                if (labels == null)
        //                {
        //                    AddressableLog($"AddressableBuildLabels Not Found");
        //                    return;
        //                }

        //                labelNames = labels.Labels;
        //            }
        //            else
        //            {
        //                labelNames = customLabels;
        //            }

        //            int count = labelNames.Count;
        //            int downLoadCount = 0;

        //            System.Action downloadCallback = () =>
        //            {
        //                ++downLoadCount;
        //                if (downLoadCount >= count)
        //                {
        //                    CompleteAll();
        //                }
        //            };
        //            for (int i = 0; i < count; i++)
        //            {
        //                string label = labelNames[i];
        //                AddressableLog($"DownLabel : {label}");
        //                var handle = Addressables.GetDownloadSizeAsync(label);
        //                this.OnDownloadDependencies?.Invoke(handle);

        //#if USE_ADDRESSABLE_TASK
        //                await handle.Task; 
        //#else
        //                await handle.ToUniTask();
        //#endif

        //                OnCompletedLoad += downloadCallback;

        //                DownLoadAddressables(handle, label);

        //            }

        //        }

        //        async void DownLoadAddressables(AsyncOperationHandle<long> completedHandler, string label)
        //        {
        //            if (completedHandler.Status != AsyncOperationStatus.Succeeded)
        //            {
        //                AddressableLog("Failed", Color.red);
        //                return;
        //            }

        //            if (completedHandler.Result < 1)
        //            {
        //                AddressableLog("handle.Result < 1", Color.green);
        //                CompletedLoad();
        //#if UNITY_EDITOR
        //                Editor.AddressableManagingDataManager.ClearData();
        //#endif
        //                Addressables.Release(completedHandler);
        //                return;
        //            }

        //            Addressables.Release(completedHandler);


        //            var handler = Addressables.DownloadDependenciesAsync(label);
        //            this.OnDownload?.Invoke(handler);

        //#if USE_ADDRESSABLE_TASK
        //            await handler.Task;
        //#else
        //            await handler.ToUniTask();
        //#endif
        //            DownloadAddressable(handler);
        //            Addressables.Release(handler);
        //            return;
        //        }

        #endregion

        private HashSet<object> CheckLabels(List<string> customLabels)
        {
            HashSet<object> labelNames = null;
            if (customLabels == null)
            {
                var addressableBuildLabels = AddressableBuildLabels;
                if (addressableBuildLabels == null)
                    return null;
                labelNames = addressableBuildLabels.LabelsObjectSet;
            }
            else
            {
                labelNames = new HashSet<object>(customLabels);
            }

            return labelNames;
        }



        void CompletedLoad()
        {
            this.OnCompletedLoad?.Invoke();
            this.OnCompletedLoad = null;
        }

        private void CompleteAll()
        {
            OnAllCompletedLoad?.Invoke();
            OnAllCompletedLoad = null;
            OnDownload = null;
            OnCompletedLoad = null;
        }

        private void ExecuteError(string errorLog)
        {
            AddressableLog(errorLog, Color.red);
            OnDownLoadError?.Invoke(new AddressableDownLoadError()
            {
                state = AddressableDownLoadState.CheckForCatalogUpdates,
                log = errorLog,
            });
        }
    }

}