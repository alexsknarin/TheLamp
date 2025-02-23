using UnityEngine.Pool;

public interface IPoolableFEnemy
{
    public void SetObjectPool(ObjectPool<FEnemy> pool);
    public void ReturnToPool();
}
