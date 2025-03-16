using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFramework.Addressable;
using UnityFramework.UI;

namespace UnityFramework.UI
{
    public partial class UIManager
    {
        public async void AddressableShow<T>(object key, System.Action<T> showComplete = null) where T : UIBase
        {
            T ui = null;
            if (!TryGetCachedUI(out ui))
            {
                AddressableResource<T> addressableResource = AddressableManager.Instance.LoadAsset<T>(key);
                await addressableResource.Task;
                T prb = addressableResource.GetResource();
                ui = GameObject.Instantiate<T>(prb);
                uis[typeof(T)] = ui;
            }

            UIController uIController = GetUIController();
            uIController.Initialize(ui);
            uIController.Show();
            showUIStack.Push(uIController);
            showComplete?.Invoke(ui);
            return;
        }


    }

}