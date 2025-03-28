using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityFramework;

public class Vector2CureTest : MonoBehaviour
{
    [SerializeField] Vector2CurveExtended curve;
    [SerializeField] AnimationCurve curves;
    [SerializeField] float time = 0.0f;
   
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

    private void OnDrawGizmos()
    {
        if (curve == null)
            return;


        float time = 0.0f;
        int segment = 1000;
        float t = 1 / segment;
        Vector3 start = curve.Evaluate(0.0f);
        for (int i = 1; i < segment; i++)
        {
            time += t * i;
            Vector3 end = curve.Evaluate(time);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);
            end = start;
        }
    }
}
