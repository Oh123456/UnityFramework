using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityFramework;

public class Vector2CureTest : MonoBehaviour
{
    [SerializeField] Vector2CurveExtended curve;
    [SerializeField] AnimationCurve curves;
    [SerializeField] float time = 0.0f;
    [SerializeField] protected Vector2CurveKeyFrame[] test;
    private void Start()
    {
        StartCoroutine(DrawLine());
    }

    private void Update()
    {
        time += Time.deltaTime;
        transform.position = curve.Evaluate(time);
    }

    IEnumerator DrawLine()
    {
        Vector3 start = transform.position;
        while (true)
        {
            yield return null;

            Debug.DrawLine(start, transform.position, Color.red , 10.0f);
            start = transform.position;
        }
    }
}
