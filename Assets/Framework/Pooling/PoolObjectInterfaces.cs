namespace UnityFramework.PoolObject
{
    public interface IPoolObject
    {
        public bool IsValid();
        public void Activate();
        public void Deactivate();
    }

    public interface IMonoPoolObject : IPoolObject
    {
        public int KeyCode { get; set; }
    }
}