using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace UnityFramework.UI
{
    interface ISubUIBase
    {
        public void Show();

        public void Hide();
        UIBase FindRootUIBase();
    }

    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
    public class SubUIBase : UIBase, ISubUIBase
    {
        enum ShowType
        {
            Auto,
            Custom,
        }

        [SerializeField] UIBase root;
        [SerializeField][Tooltip("Auto ¡æ Automatically shows when the Root is shown. \nCustom ¡æ Remains hidden even when the Root is shown; the user must manually show it.")] ShowType showType = ShowType.Auto;

        protected override void Reset()
        {
            base.Reset();
            root = FindRootUIBase();
        }

        private void Start()
        {
            if (showType == ShowType.Auto)
                root.OnShow += base.Show;
            root.OnHide += base.Hide;
        }

        public new void Show()
        {
            base.Show();
        }

        public new void Hide()
        {
            base.Hide();
        }

        public UIBase FindRootUIBase()
        {
            return UIUtils.FindParentUIBase(transform as RectTransform);
        }
    }
}
