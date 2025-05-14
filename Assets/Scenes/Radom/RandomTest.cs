using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

using UnityFramework.Algorithm;
using UnityFramework.Random;

public class RandomTest : MonoBehaviour
{
    [BurstCompile]
    public struct WeightedRandomJob : IJobParallelFor , System.IDisposable
    {
        [ReadOnly] public NativeArray<int> weights;
        [ReadOnly] public NativeArray<int> sumWeights;
        public NativeArray<int> indexs;
        public int totalWeight;
        public uint seed;

        public void Dispose()
        {
            weights.Dispose(); 
            sumWeights.Dispose();
            indexs.Dispose();
        }

        public void Execute(int index)
        {
            uint uSeed = (uint)(seed * (17 + index));
            indexs[index] = (WeightedRandom.RandomJobSystem(weights, sumWeights, totalWeight, uSeed));
        }
    }


    int totalTracking = 0;
    int minTracking = int.MaxValue;
    int maxTracking = 0;


    const int MAX_VALUE = int.MaxValue / 2;
    class Data
    {
        public int currentWeight;
        public double percent;
        public double currentPercent;
        public int count;

        public void Update(long total)
        {
            if (count == 0)
                return;
            currentPercent = (double)((double)count / (double)total);
        }

    }

    [SerializeField]
    int[] rarityWeight = new int[]
        {
            3,12,85
        };

    [SerializeField] int sampleCount = 1000000;
    [SerializeField] int weightCount = 100000;
    [SerializeField] List<int> weights;
    [SerializeField] long totalCount = 0;
    [SerializeField] TMPro.TMP_Text text;
    List<Data> datas;

    bool isInit = false;
    private void Start()
    {
        weights = new List<int>(weightCount);
        datas = new List<Data>(weightCount);

        int[] testarray = new int[weightCount];

        int total = 0;
        for (int i = 0; i < weightCount; i++)
        {
            int current = Random.Range(1, 200);
            total += current;
            weights.Add(current);
            testarray[i] = total;
        }

        for (int i = 0; i < weightCount; i++)
        {
            datas.Add(new Data()
            {
                currentWeight = weights[i],
                percent = (float)weights[i] / (float)total,
                currentPercent = 0.0f
            });
        }
        isInit = true;



        //StartCoroutine(ZZZZ(() =>
        //{
        //    int count =  Random.Range(0, total);
        //
        //    if (testarray.UpperBound(weightCount, count) != WeightedRandom.UpperBound(testarray, weightCount, count))
        //        Debug.Log("문제발생");
        //    else
        //        Debug.Log("같음");
        //
        //}));

        TestF();
    }


    IEnumerator ZZZZ(System.Action zzzz)
    {
        while (true)
        {
            zzzz();

            yield return null;  
        }
    }

    void TestF()
    {

        NativeArray<int> weights = new NativeArray<int>(weightCount,Allocator.TempJob);
        int totalWeight = 0;

        for(int i = 0; i < weightCount; i++)
        {
            weights[i] = this.weights[i];
            totalWeight += this.weights[i];
        }

        NativeArray<int> array = new NativeArray<int>(weightCount, Allocator.TempJob);
        array[0] = weights[0];
        for (int i = 1; i < weightCount; i++)
        {
            array[i] = array[i - 1] + weights[i];
        }

        WeightedRandomJob job = new WeightedRandomJob()
        {
            weights = weights,
            sumWeights = array,
            totalWeight = totalWeight,
            seed = (uint)(System.Diagnostics.Stopwatch.GetTimestamp() * 31),
            indexs = new NativeArray<int>(sampleCount, Allocator.TempJob)
        };

        Debug.Log("Start Job");

        var handle = job.Schedule(sampleCount, 64);
        handle.Complete();
        Debug.Log("End Job");
        totalCount = sampleCount;

        for (int i = 0; i < sampleCount; i++)
        {
            var data = datas[job.indexs[i]];
            data.count++;
        }
        text.text = "";
        string Title = $"Total Count {totalCount} log({weightCount}) = {Mathf.Log(weightCount, 2):F2}";
        double minAccuracy = 100.0;
        int minIndex = 0;
        for (int i = 0; i < weightCount; i++)
        {
            var data = datas[i];
            data.Update(totalCount);


            double relativeError = System.Math.Abs(data.currentPercent - data.percent) / data.percent * 100.0;
            double accuracy = (1.0 - System.Math.Min(1f, relativeError / 100.0)) * 100.0;

            if (minAccuracy > accuracy)
            {
                minAccuracy = accuracy;
                minIndex = i;
            }

            text.text = $"{text.text} \n index : {i} (Weight {data.currentWeight}) :  {data.currentPercent:F7} / {data.percent:F7} (Accuracy {accuracy:F1} %)";
        }

        text.text = $"{Title} MinAccuracy {minAccuracy} Index {minIndex} {text.text}";


        job.Dispose();
    }



    private void Update()
    {
        return;
        if (!isInit) return;

        if (totalCount >= MAX_VALUE)
            return;

        int index = WeightedRandom.Random(weights);

        try
        {
            totalCount++;
            var data = datas[index];
            data.count++;


            text.text = $"Total Count {totalCount} log({weightCount}) = {Mathf.Log(weightCount,2):F2}";


            for (int i = 0; i < weightCount; i++)
            {
                data = datas[i];
                data.Update(totalCount);

                double relativeError = System.Math.Abs(data.currentPercent - data.percent) / data.percent * 100;
                double accuracy = (1f - System.Math.Min(1.0, relativeError / 100)) * 100;

                text.text = $"{text.text} \n index : {i} (Weight {data.currentWeight}) :  {data.currentPercent:F7} / {data.percent:F7} (Accuracy {accuracy:F1} %)";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"index :{index}");
        }
    }


}
