using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFramework.UI
{

    public struct UIUtils
    {
        public static UIBase FindParentUIBase(RectTransform rectTransform)
        {
            Transform currentParent = rectTransform.parent;
            UIBase uIBase = null;
            while (!currentParent.TryGetComponent<UIBase>(out uIBase))
            {
                currentParent = currentParent.parent;
                if (currentParent == null)
                    break;
            }

            return uIBase;
        }
    }
}