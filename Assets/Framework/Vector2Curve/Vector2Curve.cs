using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;

namespace UnityFramework
{
    [System.Serializable]
    public struct Vector2CurveKeyFrame
    {
        public Vector2 curvePosition;
        public Vector2 inHandle;
        public Vector2 outHandle;
    }

    [System.Serializable]
    public class Vector2Curve
    {
        public enum CurveMode
        {
            Bezier,
            Catmull_Rom,
        }


        [SerializeField] protected Vector2[] moveCurves = new Vector2[] { -Vector2.one, Vector2.one };
        [SerializeField] protected CurveMode curveMode = CurveMode.Catmull_Rom;
        protected Vector2 lastPosint;
        

        public Vector2[] MoveCurves => this.MoveCurves;


        public Vector2Curve()
        {
            lastPosint = moveCurves[moveCurves.Length - 1];
        }


        public static Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2 * p1) +
                (-p0 + p2) * t +
                (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                (-p0 + 3 * p1 - 3 * p2 + p3) * t3
            );
        }

        public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float ttt = tt * t;
            float uuu = uu * u;

            return uuu * p0
                 + 3 * uu * t * p1
                 + 3 * u * tt * p2
                 + ttt * p3;
        }

        public static Vector2 EvaluateCatmullRom(float t, in Vector2[] moveCurves)
        {

            int length = moveCurves.Length;
            int ratio = length - 1;

            float realValue = t * ratio;

            int index = (int)realValue;

            Vector2 p0 = moveCurves[index];
            Vector2 p1 = moveCurves[index];
            Vector2 p2 = moveCurves[index + 1];
            Vector2 p3 = moveCurves[index + 1];


            return CatmullRom(p0, p1, p2, p3, realValue - index);
        }


        public static Vector2 EvaluateCubicBezier(float t, in Vector2[] moveCurves)
        {


            int length = moveCurves.Length;
            int ratio = length - 1;

            float realValue = t * ratio;

            int index = (int)realValue;

            Vector2 p0 = (index == 0) ? moveCurves[index] : moveCurves[index - 1];
            Vector2 p3 = (index == length - 2) ? moveCurves[index + 1] : moveCurves[index + 2];

            Vector2 delta = p3 - p0;
            float distance = (delta).sqrMagnitude;

            float scale = 0.25f;
            Vector2 offset = delta * scale;

            Vector2 p1 = p0 + offset;
            Vector2 p2 = p3 + offset;


            return CatmullRom(p0, p1, p2, p3, realValue - index);

        }




        public virtual Vector2 Evaluate(float t)
        {
            if (t >= 1.0f)
                return lastPosint;
            return Vector2Curve.EvaluateCatmullRom(t, this.moveCurves);
        }

        public bool ContainsAnyPoint() { return moveCurves.Length > 0; }


    }


}