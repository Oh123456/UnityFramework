using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityFramework.PoolObject;

namespace UnityFramework.Pool.Test
{
    public class TestMono2 : MonoPoolObject
    {
        static int count = 0;
        [SerializeField]
        int index = 0;

        public TestMono2()
        {
            index = count++;
        }

        public override void Activate()
        {
            base.Activate();
            gameObject.name = $"TestMono2({index})";
        }
    }

}