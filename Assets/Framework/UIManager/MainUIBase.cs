using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFramework.UI
{

    [RequireComponent(typeof(Canvas))]
    public class MainUIBase : UIBase
    {
        public void AddListener(UIManager.UIController uIController)
        {
            uIController.Show = Show;
            uIController.Hide = Hide;
        }
    }

}