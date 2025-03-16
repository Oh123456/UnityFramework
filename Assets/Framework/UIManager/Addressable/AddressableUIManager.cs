using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFramework.UI;

namespace UnityFramework.Addressable.UI
{
    public partial class UIManager
    {
        public async void AddressableShow<T>(object key) where T : UIBase
        {
            //Get
            AddressableResource<T> addressableResource = AddressableManager.Instance.LoadAsset<T>(key);
            await addressableResource.Task;

        }
        
    }

}