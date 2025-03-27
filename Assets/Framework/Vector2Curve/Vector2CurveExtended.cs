using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFramework
{
    [System.Serializable]
    public class Vector2CurveExtended : Vector2Curve
    {
        public enum CurvePlayeMode
        {
            Once,
            Loop,
            PingPong,
        }

        [SerializeField] CurvePlayeMode curvePlayeMode = CurvePlayeMode.Loop;

        public Vector2CurveExtended() : base()
        {

        }  

        public override Vector2 Evaluate(float t)
        {
            switch (curvePlayeMode)
            {
                case CurvePlayeMode.Loop:
                    t = t - Mathf.Floor(t);
                    break;
                case CurvePlayeMode.PingPong:

                    int cycle = (int)t;
                    float frac = t - cycle;
                    // 비트 연산으로 홀수 짝수 검사
                    t = (cycle & 1) == 1 ? 1.0f - frac : frac;
                    break;

            }

            return base.Evaluate(t);
        }

    }

}