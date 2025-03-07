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

        public event Action OnCompletedLoad;
        public event Action<AddressableDownLoadData> OnDownloadDependencies;
        public event Action<AddressableDownLoadData> OnDownload;

        List<string> labelNames;

   

#if UNITY_EDITOR
        public AddressableManager()
        {
            UnityEditor.EditorApplication.playModeStateChanged += (playModeStateChange) =>
                {
                    if (playModeStateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                    {
                        if (this.addressableDataManager.IsValueCreated)
                            this.addressableDataManager.Value.Release();
                    }
                    Debug.Log($"PlayModeStateChange : {playModeStateChange}");
                };
        }

#endif

        public void UpdateLabelNames(List<string> label)
        {
            this.labelNames = label;
        }

        public async void DownLoad()
        {
            AddressableLog("Addressables Start");

            AddressableBuildLabels labels = Resources.Load<AddressableBuildLabels>(AddressableBuildLabels.NAME);

            if (labels == null)
            {
                AddressableLog($"AddressableBuildLabels Not Found");
                return;
            }

            this.labelNames = labels.Labels;

            int count = this.labelNames.Count;
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
                Editor.AddressableManagingDataManager.addressableManagingDatas.Clear();
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