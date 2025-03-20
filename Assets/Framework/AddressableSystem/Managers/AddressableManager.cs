using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityFramework.Addressable.Managing;



#if USE_ADDRESSABLE_TASK
using System.Threading.Tasks;
#else
using Cysharp.Threading.Tasks;
#endif

namespace UnityFramework.Addressable
{
    public struct AddressableDownLoadData
    {
        public AsyncOperationHandle handle;
        public string label;
    }

    public partial class AddressableManager : Singleton.LazySingleton<AddressableManager>
    {
        public const string BUILD_LABELS_PATH = "Assets/Resources";

        public event Action OnAllCompletedLoad;
        public event Action OnCompletedLoad;
        public event Action<AddressableDownLoadData> OnDownloadDependencies;
        public event Action<AddressableDownLoadData> OnDownload;

        List<string> labelNames;



#if UNITY_EDITOR
        public AddressableManager()
        {
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
        }

#endif

        /// <summary>
        /// A function in Addressables that downloads remote asset bundles, storing them in the local cache via the network, allowing them to be loaded later. 
        /// </summary>
        /// <param name="customLabels">If the list of labels to download is set to null, all currently used labels are automatically detected, and the corresponding assets are downloaded. This allows necessary resources to be loaded without explicitly specifying labels.</param>
        public async void DownLoad(List<string> customLabels = null)
        {
            AddressableLog("Addressables Start");
            if (customLabels == null)
            {
                AddressableBuildLabels labels = Resources.Load<AddressableBuildLabels>(AddressableBuildLabels.NAME);

                if (labels == null)
                {
                    AddressableLog($"AddressableBuildLabels Not Found");
                    return;
                }

                this.labelNames = labels.Labels;
            }
            else
            {
                this.labelNames = customLabels;
            }

            int count = this.labelNames.Count;
            int downLoadCount = 0;

            System.Action downloadCallback = () =>
            {
                ++downLoadCount;
                if (downLoadCount >= count)
                {
                    OnAllCompletedLoad?.Invoke();
                    OnAllCompletedLoad = null;
                }
            };
            for (int i = 0; i < count; i++)
            {
                string label = this.labelNames[i];
                AddressableLog($"DownLabel : {label}");
                var handle = Addressables.GetDownloadSizeAsync(label);
                this.OnDownloadDependencies?.Invoke(new AddressableDownLoadData()
                {
                    handle = handle,
                    label = label
                });

#if USE_ADDRESSABLE_TASK
                await handle.Task; 
#else
                await handle.ToUniTask();
#endif

                OnCompletedLoad += downloadCallback;

                DownLoadAddressables(handle, label);

            }

        }


        async void DownLoadAddressables(AsyncOperationHandle<long> completedHandler, string label)
        {
            if (completedHandler.Status != AsyncOperationStatus.Succeeded)
            {
                AddressableLog("Failed", Color.red);
                return;
            }

            if (completedHandler.Result < 1)
            {
                AddressableLog("handle.Result < 1", Color.green);
                CompletedLoad();
#if UNITY_EDITOR
                Editor.AddressableManagingDataManager.ClearData();
#endif
                Addressables.Release(completedHandler);
                return;
            }

            Addressables.Release(completedHandler);


            var handler = Addressables.DownloadDependenciesAsync(label);
            this.OnDownload?.Invoke(new AddressableDownLoadData()
            {
                handle = handler,
                label = label
            });

#if USE_ADDRESSABLE_TASK
            await handler.Task;
#else
            await handler.ToUniTask();
#endif
            DownladAddressable(handler);
            Addressables.Release(handler);
            return;
        }


        void DownladAddressable(AsyncOperationHandle handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AddressableLog("Download Completed", Color.green);
                CompletedLoad();
                return;
            }
            AddressableLog("Download Failed", Color.red);
            return;
        }

        void CompletedLoad()
        {
            this.OnCompletedLoad?.Invoke();
            ClearEvents();
        }

        public void ClearEvents()
        {
            this.OnCompletedLoad = null;
            this.OnDownloadDependencies = null;
            this.OnDownload = null;
        }

    }

}