using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityFramework.PoolObject;

namespace UnityFramework.Pool.Test
{
	public class PoolTester : MonoBehaviour
	{
		[SerializeField] TestMono testMono;
		[SerializeField] TestMono testMono2;
		[SerializeField] TestMono2 testMono3;
		[SerializeField] TestMono2 testMono4;

        [SerializeField] int[] array;

        Queue<TestMono> testMonos = new Queue<TestMono>();
        Queue<TestMono> testMonos2 = new Queue<TestMono>();
        Queue<TestMono2> testMonos3 = new Queue<TestMono2>();
        Queue<TestMono2> testMonos4 = new Queue<TestMono2>();

        Queue<TestClass> testClasses = new Queue<TestClass>();
        Queue<TestClass2> testClasses2 = new Queue<TestClass2>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                testMonos.Enqueue(PoolManager.GetMonoObject<TestMono>(testMono));
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (testMonos.Count == 0)
                    return;

                PoolManager.SetMonoObject<TestMono>(testMonos.Dequeue());   
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                testMonos2.Enqueue(PoolManager.GetMonoObject<TestMono>(testMono2));
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (testMonos2.Count == 0)
                    return;

                PoolManager.SetMonoObject<TestMono>(testMonos2.Dequeue());
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                testMonos3.Enqueue(PoolManager.GetMonoObject<TestMono2>(testMono3));
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                if (testMonos3.Count == 0)
                    return;

                PoolManager.SetMonoObject<TestMono2>(testMonos3.Dequeue());
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                testMonos4.Enqueue(PoolManager.GetMonoObject<TestMono2>(testMono4));
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (testMonos4.Count == 0)
                    return;

                PoolManager.SetMonoObject<TestMono2>(testMonos4.Dequeue());
            }


            if (Input.GetKeyDown(KeyCode.H))
            {
                testClasses.Enqueue(PoolManager.GetClassObject<TestClass>());
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (testClasses.Count == 0)
                    return;

                PoolManager.SetClassObject(testClasses.Dequeue());
            }



            if (Input.GetKeyDown(KeyCode.F))
            {
                testClasses2.Enqueue(PoolManager.GetClassObject<TestClass2>());
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                if (testClasses2.Count == 0)
                    return;

                PoolManager.SetClassObject(testClasses2.Dequeue());
            }


            if (Input.GetKeyDown(KeyCode.Q))
            {
                ArrayPoolObject<int> arrayPoolObject = PoolManager.GetArray<int>(array.Length, true);
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        arrayPoolObject[i] = array[i];
                    }
                }

                arrayPoolObject.Dispose();
            }
        }
    }

}