using UnityEngine;

public abstract class DamageIndication : MonoBehaviour, IInitializable
{
    public abstract void Initialize();
    public abstract void StartPerform();
}
