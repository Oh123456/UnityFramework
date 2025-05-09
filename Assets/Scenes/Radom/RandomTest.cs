using System.Collections;
using System.Collections.Generic;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

using UnityEngine;

using UnityFramework.Random;

public class RandomTest : MonoBehaviour
{


    class Data
    {
        public int currentWeight;
        public float percent;
        public float currentPercent;
        public int count;

        public void Update(long total)
        {
            if (count == 0)
                return;
            currentPercent = (float)((float)count/ (double)total);
        }

    }

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

        int total = 0;
        for (int i = 0; i < weightCount; i++)
        {
            int current = Random.Range(1, 200);
            total += current;
            weights.Add(current);
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

    }
    private void Update()
    {
        if (!isInit) return;


        int index = WeightedRandom.Random(weights);

        totalCount++;
        var data = datas[index];
        data.count++;

        text.text = $"Total Count {totalCount}";

        for (int i = 0; i < weightCount; i++)
        {
            data = datas[i];
            data.Update(totalCount);

            text.text = $"{text.text} \n index : {i} :  {data.currentPercent} / {data.percent}";
        }
    }


}
