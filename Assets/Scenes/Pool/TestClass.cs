using UnityEngine;
using UnityFramework.PoolObject;

namespace UnityFramework.Pool.Test
{
    public class TestClass : ClassPoolObject
    {
        static int count = 0;
        int index = 0;

        public TestClass()
        {
            index = count++;
            Debug.Log($"생성 {index}");
        }

        public override void Activate()
        {
            base.Activate();
            Debug.Log($"활성화 {index}");
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Debug.Log($"비활성화 {index}");
        }
    }

    public class TestClass2 : ClassPoolObject
    {
        static int count = 0;
        int index = 0;

        public TestClass2()
        {
            index = count++;
            Debug.Log($"2 생성 {index}");
        }

        public override void Activate()
        {
            base.Activate();
            Debug.Log($"2 활성화 {index}");
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Debug.Log($"2 비활성화 {index}");
        }
    }
}