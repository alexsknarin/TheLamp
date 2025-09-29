using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.InGamePresentation.CameraShake
{
    public abstract class BaseCameraShakeStrategy: ScriptableObject, IStrategy, IInitializable
    {
        public abstract void Construct(Transform bossTransform);
        public abstract void Initialize();
        public abstract Vector3 Execute();
    }
}
