using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFramework.UI
{
	[RequireComponent(typeof(Canvas)), RequireComponent(typeof(GraphicRaycaster))]
	public class SubUIBase : UIBase
	{
		public new void Show()
		{ 
			base.Show();
		}

        public new void Hide()
        {
            base.Hide();
        }
    } 
}
