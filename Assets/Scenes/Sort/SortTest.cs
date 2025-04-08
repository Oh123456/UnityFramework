using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityFramework.Algorithm;

public class SortTest : MonoBehaviour
{
    [SerializeField] int randomCount = 200;
    [SerializeField] int[] array;
    [SerializeField] int[] result;


    private void Start()
    {
        StartCoroutine(FUCK());
    }


    IEnumerator FUCK()
    {
        while (true)
        {
            Test();
            yield return null;  
        }
    }


    private void Test()
    {
        array = new int[randomCount];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Random.Range(0, int.MaxValue);
        }

        result = new int[randomCount];

        int[] temp = new int[randomCount];
        for (int i = 0; i < array.Length; i++)
        {
            temp[i] = array[i];
        }

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        temp.MakeHeap();

        sw.Stop();
        Debug.Log($"실행 시간: {sw.ElapsedMilliseconds}ms");

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = temp[i];
        }


        int maxIndex = 0;
        int minIndex = 0;

        for (int i = 1; i < array.Length; i++)
        {
            if (result[maxIndex] < result[i])
                maxIndex = i;

            if (result[minIndex] > result[i])
                minIndex = i;
        }

        Debug.Log($"MaxIndex {maxIndex} MinIndex {minIndex} test {ArrayAlgorithm.testValue}");
    }
}
