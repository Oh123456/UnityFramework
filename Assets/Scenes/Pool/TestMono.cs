using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFramework.PoolObject;

namespace UnityFramework.Pool.Test
{
    public class TestMono : MonoPoolObject
    {
        static int count = 0;
        [SerializeField]
        int index = 0;

        public TestMono()
        {
            index = count++;
        }


        public override void Activate()
        {
            base.Activate();
            gameObject.name = $"TestMono({index})";
        }
    }

}