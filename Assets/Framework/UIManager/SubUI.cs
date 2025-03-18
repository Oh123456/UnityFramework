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
        public bool IsIndependent();
        UIBase FindRootUIBase();
    }

	[RequireComponent(typeof(Canvas))]
	public class SubUI : UIBase , ISubUIBase
    {
        enum ShowType
        {
            Auto,
            Custom,
        }

        [SerializeField] UIBase root;
        [SerializeField][Tooltip("Auto → Automatically shows when the Root is shown. \nCustom → Remains hidden even when the Root is shown; the user must manually show it.")] ShowType showType = ShowType.Auto;

        protected override void Reset()
        {
            base.Reset();
            root = FindRootUIBase();
        }

        private void Start()
        {
            // 독립 캔버스가 아니면 같이 꺼짐으로 의미 X
            if (!canvas.overrideSorting)
                return;

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
            return UIUtils.FindParentIndependentUIBase(transform as RectTransform);
        }

        public bool IsIndependent()
        {
            return canvas.overrideSorting;  
        }
    } 
}
 