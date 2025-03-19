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
            Debug.Log($"���� {index}");
        }

        public override void Activate()
        {
            base.Activate();
            Debug.Log($"Ȱ��ȭ {index}");
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Debug.Log($"��Ȱ��ȭ {index}");
        }
    }

    public class TestClass2 : ClassPoolObject
    {
        static int count = 0;
        int index = 0;

        public TestClass2()
        {
            index = count++;
            Debug.Log($"2 ���� {index}");
        }

        public override void Activate()
        {
            base.Activate();
            Debug.Log($"2 Ȱ��ȭ {index}");
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Debug.Log($"2 ��Ȱ��ȭ {index}");
        }
    }
}