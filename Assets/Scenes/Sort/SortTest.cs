using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityFramework.Algorithm;
using UnityFramework.Collections;

public class SortTest : MonoBehaviour
{
    [SerializeField] int randomCount = 200;
    [SerializeField] int[] array;
    [SerializeField] int[] result;

    PriorityQueue<int> test;

    private void Start()
    {
        StartCoroutine(FUCK());

        //test = new PriorityQueue<int>();
        //StartCoroutine(FUCK2());


    }


    IEnumerator FUCK()
    {
        while (true)
        {
            Test();
            yield return null;  
        }
    }

    IEnumerator FUCK2()
    {
        for (int i = 0; i < randomCount; i++)
        {
            test.Enqueue(Random.Range(0, int.MaxValue - 10));
        }

        int index = 0;
        foreach (var i in test)
        {
            Debug.Log($"index {index} : {i}");   
        }


        while (true)
        {
            Test2();
            yield return null;
        }
    }

    private void Test2()
    {

        //int maxValue = test.Dequeue();
        //int count = 0;
        //while (!test.Empty())
        {
            var testValue = test.Dequeue();
            Debug.Log($"max{testValue}");
            //if (maxValue < testValue)
            //{
            //    Debug.Log("망함");
            //}

            //test.Enqueue(int.MaxValue);
            test.Enqueue(Random.Range(0, int.MaxValue));
            //if (count > 100)
            //{
            //    for (int i = 0; i < 99; i++)
            //    {
            //        test.Enqueue(Random.Range(0, int.MaxValue - 10));
            //    }

            //    test.Enqueue(int.MaxValue);
            //}
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

        temp.MaekAndHeapSort();

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
    }
}
