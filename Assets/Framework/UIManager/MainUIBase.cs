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
        /// ?ㅻ줈媛湲?踰꾪듉???뚮??꾨뻹
        /// </summary>
        /// <returns>true 諛섑솚???ㅻ줈媛湲??깃났 false ?쇱떆 ?ㅻⅨ 臾댁뼵媛媛 ???爰쇱졇?쇳븯???곹솴</returns>
        public virtual bool ExecuteButton()
        {
            return true;
        }
    }

}