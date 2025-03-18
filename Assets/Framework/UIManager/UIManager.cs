using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFramework.Singleton;

namespace UnityFramework.UI
{
    public partial class UIManager : LazySingleton<UIManager>
    {
        public class UIController 
        {
            MainUIBase mainUIBase;

            public System.Action Show;
            public System.Action Hide;

            public MainUIBase MainUIBase => this.mainUIBase;

            public void Initialize(MainUIBase uIBase)
            {
                this.mainUIBase = uIBase;
                this.mainUIBase.AddListener(this);
            }

            public void Release()
            {
                mainUIBase = null;
                Show = null;
                Hide = null;                
            }

        }

        private Dictionary<System.Type, UIBase> uis = new Dictionary<System.Type, UIBase>();
        private Stack<UIController> showUIStack = new Stack<UIController>(4);
        private Stack<UIController> controllerPool = new Stack<UIController>(4);


        public T Show<T>(string name, int sortOrder = 0) where T : MainUIBase
        {
            T ui = GetCachedUI<T>(name);
            ui.SetSortOrder(sortOrder);

            UIController uIController = GetUIController();
            uIController.Initialize(ui);
            uIController.Show();
            showUIStack.Push(uIController);
            return ui;
        }

        public void Hide()
        {

            if (GetActiveUIController(out UIController uIController))
            {
                uIController.Hide();
                uIController.Release();
                controllerPool.Push(uIController);
            }
          
        }

        public void Close()
        {
            if (GetActiveUIController(out UIController uIController))
            {
                uIController.Hide();
                uIController.MainUIBase.Close();
                uIController.Release();
                controllerPool.Push(uIController);
            }
        }

        private bool GetActiveUIController(out UIController uIController)
        {
            uIController = null;
            if (showUIStack.Count == 0)
                return false;
            uIController = showUIStack.Pop();
            return true;
        }

        public bool TryGetCachedUI<T>(out T ui) where T : MainUIBase
        {
            bool result = uis.TryGetValue(typeof(T), out var baseUI);
            if (baseUI == null)
                result = false;
            ui = result ? (T)baseUI : null;
            return result;
        }

        private T GetCachedUI<T>(string name) where T : MainUIBase
        {
            T ui = null;
            if (!TryGetCachedUI(out ui))
            {
                T prb = Resources.Load<T>(name);
                ui = GameObject.Instantiate<T>(prb);
                uis[typeof(T)]= ui;
            }

            return ui;
        }

        private UIController GetUIController()
        {
            if (controllerPool.Count == 0)
                return new UIController();

            return controllerPool.Pop();
        }
    }
}
