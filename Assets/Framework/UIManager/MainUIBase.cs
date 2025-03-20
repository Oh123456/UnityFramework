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

        /// <summary>
        /// 뒤로가기 버튼을 눌렀을떄
        /// </summary>
        /// <returns>true 반환시 뒤로가기 성공 false 일시 다른 무언가가 대신 꺼져야하는 상황</returns>
        public virtual bool ExecuteButton()
        {
            return true;
        }
    }

}