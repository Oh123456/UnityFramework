using System.Buffers;
using System.Collections.Generic;

namespace UnityFramework.Random
{
    public static class WeightedRandom
    {
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

            int index = LowerBound(array, count, UnityEngine.Random.Range(0, totalWeight + 1));
            ArrayPool<int>.Shared.Return(array, clearArray: true);
            return index;

        }


        private static int LowerBound(int[] array ,int count, int target)
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

    } 
}
