using UnityEngine;

[System.Serializable]
public class Vector2Curve 
{
    [SerializeField] Vector2[] moveCurves = new Vector2[] { -Vector2.one, Vector2.one };

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

    public static Vector2 Evaluate(float t, in Vector2[] moveCurves)
    {
        float value = Mathf.Clamp01(t);

        int ratio = moveCurves.Length - 1;

        float realValue = t * ratio;

        int index = (int)realValue;

        Vector2 p0 = (index == 0) ? moveCurves[index] : moveCurves[index - 1];
        Vector2 p1 = moveCurves[index];
        Vector2 p2 = moveCurves[index + 1];
        Vector2 p3 = (index == moveCurves.Length - 2) ? moveCurves[index + 1] : moveCurves[index + 2];


        return CatmullRom(p0, p1, p2, p3, realValue % 1.0f);
    }

    public Vector2 Evaluate(float t)
    {
        return Vector2Curve.Evaluate(t, this.moveCurves);
    }

}

