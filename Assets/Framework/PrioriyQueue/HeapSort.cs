using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

namespace UnityFramework.Algorithm
{
    public static class ArrayAlgorithm
    {
        public static int testValue = 0;
        public static void HeapSort<T>(this T[] array)
        {
            testValue = 0;
            IComparer<T> comparer = Comparer<T>.Default;

            MakeHeap(array, comparer);

            int length = array.Length;
            // 다시 재정렬
            for (int i = length - 1; i > 0; i--)
            {
                array.Swap(0, i);
                Heapify(array, 0, i, comparer);
            }
        }

        public static void HeapSort<T>(this T[] array, IComparer<T> comparer)
        {
            MakeHeap(array, comparer);

            int length = array.Length;
            // 다시 재정렬
            for (int i = length - 1; i > 0; i--)
            {
                array.Swap(0, i);
                Heapify(array, i, length, comparer);
            }
        }


        public static void MakeHeap<T>(this T[] array)
        {
            testValue = 0;
            IComparer<T> comparer = Comparer<T>.Default;

            int length = array.Length;

            for (int i = (length / 2) - 1; i >= 0; i--)
            {
                Heapify(array, i, length, comparer);
            }

        }

        public static void MakeHeap<T>(this T[] array, IComparer<T> comparer)
        {
            int length = array.Length;

            for (int i = (length / 2) - 1; i >= 0; i--)
            {
                Heapify(array, i, length, comparer);
            }

        }


        public static void Swap<T>(this T[] array, int index, int index2)
        {
            T temp = array[index];
            array[index] = array[index2];
            array[index2] = temp;
        }


        private static void Heapify<T>(T[] array, int rootIndex, int heapSize ,IComparer<T> comparer)
        {
            testValue++;

            int largest = rootIndex;
            int left = 2 * rootIndex + 1;
            int right = left + 1;


            // 왼쪽이랑 검사해서 -1 즉 먼저오는게 나오면 큰거로 저장
            if (left < heapSize && comparer.Compare(array[left], array[largest]) > 0)
                largest = left;

            // 오른쪽이랑 검사해서 -1 즉 먼저오는게 나오면 큰거로 저장
            if (right < heapSize && comparer.Compare(array[right], array[largest]) > 0)
                largest = right;

            if (largest != rootIndex)
            {
                array.Swap(rootIndex, largest);
                Heapify(array, largest ,heapSize, comparer);
            }


        }


    }
}
