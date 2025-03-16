using _GAME.Scripts.Enemies;
using UnityEngine.Pool;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IPoolableFEnemy
    {
        public void SetObjectPool(ObjectPool<FEnemy> pool);
        public void ReturnToPool();
    }
}
