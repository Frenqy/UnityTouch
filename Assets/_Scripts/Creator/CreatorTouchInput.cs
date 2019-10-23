using System.Collections;
using System.Collections.Generic;
using TouchScript;
using UnityEngine;

namespace VIC.Creator
{

    public class CreatorTouchInput : TouchInput
    {
        public override void OnEnable()
        {
            base.OnEnable();
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersUpdated += OnPressedHandler;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (TouchManager.Instance != null)
            {
                TouchManager.Instance.PointersUpdated -= OnPressedHandler;
            }
        }

        private void OnPressedHandler(object sender, PointerEventArgs e)
        {
            ChooseFirstTriangle();
        }

        bool testLog = true;

        private void ChooseFirstTriangle()
        {
            if(testLog)
            {
                //Debug.LogError("ChooseFirstTriangle");
                testLog = false;
            }
            if (Triangels.Count > 0)
            {
                Debug.LogError(Triangels.Count);
                Triangel firstOne = Triangels[0];
            }
        }
    }
}
