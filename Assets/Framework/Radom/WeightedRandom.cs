using System;
using System.Buffers;
using System.Collections.Generic;

using Unity.Collections;

namespace UnityFramework.Random
{
    public static class WeightedRandom
    {
        public static int tracking = 0;

        public static int Random(List<int> weightList)
        {
            int count = weightList.Count;
            int totalWeight = 0;
            for (int i = 0 ; i < count; i++)
            {
                totalWeight += weightList[i];
            }


            int[] array = ArrayPool<int>.Shared.Rent(count);
            array[0] = weightList[0];
            for (int i = 1; i < count; i++)
            {
                array[i] = array[i - 1] + weightList[i];
            }

            uint seed = (uint)System.Diagnostics.Stopwatch.GetTimestamp();
            Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
            //int index = UpperBound(array, count, UnityEngine.Random.Range(0, totalWeight));
            int index = UpperBound(array, count, random.NextInt(0,totalWeight));
            ArrayPool<int>.Shared.Return(array, clearArray: true);
            return index;

        }

        public static int Random(NativeArray<int> weightList, NativeArray<int> sumArray, int totalWeight, uint seed)
        {
            int count = weightList.Length;
            Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
            int index = UpperBound(sumArray, count, random.NextInt(0, totalWeight));
            return index;

        }


        private static int UpperBound(NativeArray<int> array, int count, int target)
        {
            int low = 0;
            int high = count;
            int mid = 0;

            while (low < high)
            {
                mid = (low + high) / 2;

                if (array[mid] <= target)
                    low = mid + 1;
                else
                    high = mid;
            }

            return low;
        }

        private static int UpperBound(int[] array ,int count, int target)
        {
            tracking = 0;
            int low = 0;
            int high = count;
            int mid = 0;

            while (low < high)
            {
                mid = (low + high) / 2;

                if (array[mid] <= target)
                    low = mid + 1;
                else
                    high = mid;
                tracking++;
            }

            return low;
        }

    } 
}
