using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityFramework.UI
{

    public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler 
    {
        [SerializeField] UIBase cotnrolUIbase;

        private void Reset()
        {
            cotnrolUIbase = UIUtils.FindParentUIBase(transform as RectTransform);
        }

        private void Start()
        {
            if (cotnrolUIbase == null)
                Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (cotnrolUIbase is ISubUIBase subUIBase)
            {
                subUIBase.Hide();
                return;
            }
            UIManager.Instance.Hide();
        }
    }

}