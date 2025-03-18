using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFramework.UI
{
    [RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
    public class UIBase : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster graphicRaycaster;

        protected bool isShow = false;

        public event System.Action OnShow;
        public event System.Action OnHide;
        public event System.Action OnClose;

        public bool IsClosed => !gameObject.activeSelf;

        private void Reset()
        {
            canvas = GetComponent<Canvas>();
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        protected virtual void Show()
        {
            canvas.enabled = true;
            graphicRaycaster.enabled = true;
            isShow = true;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            OnShow?.Invoke();   
        }

        protected virtual void Hide()
        {
            canvas.enabled = false;
            graphicRaycaster.enabled = false;
            isShow = false;
            OnHide?.Invoke();
        }

        public virtual void Close()
        {
            if (isShow)
                return;
            gameObject.SetActive(false);
            OnClose?.Invoke();
        }

        public void SetSortOrder(int oreder)
        {
            canvas.sortingOrder = oreder;
        }

        public void AddListener(UIManager.UIController uIController)
        {
            uIController.Show = Show;
            uIController.Hide = Hide;
        }
    }

}