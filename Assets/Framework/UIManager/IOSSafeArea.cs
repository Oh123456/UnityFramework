using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFramework.UI
{
    public class SafeArea : MonoBehaviour
    {
#if UNITY_EDITOR || UNITY_IOS
        RectTransform rectTransform;
        [SerializeField] bool ignoreBottom = false;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }


        void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            if (ignoreBottom)
                anchorMin.y = 0.0f;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;



            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

#endif
    }

}