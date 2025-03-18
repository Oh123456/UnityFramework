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

        public static UIBase FindParentIndependentUIBase(RectTransform rectTransform)
        {
            Transform currentParent = rectTransform.parent;
            UIBase uIBase = null;


            Transform temp = null;
            while (true)
            {
                temp = currentParent;

                currentParent = currentParent.parent;
                if (currentParent == null)
                {
                    uIBase = temp.GetComponent<UIBase>();
                    break;
                }

                if (!temp.TryGetComponent<UIBase>(out uIBase))
                    continue;
                

                if (uIBase is ISubUIBase subUIBase)
                {
                    if (!subUIBase.IsIndependent())
                        continue;
                }


                break;
            }

            return uIBase;
        }
    }
}