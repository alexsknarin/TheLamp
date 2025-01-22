using UnityEngine;

public abstract class BaseCameraShakeStrategy: ScriptableObject, ICameraShakeStrategy, IInitializable
{
    public abstract void Construct(Transform bossTransform);
    public abstract void Initialize();
    public abstract Vector3 Execute();
}
