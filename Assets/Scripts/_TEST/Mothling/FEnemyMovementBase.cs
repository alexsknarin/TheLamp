using UnityEngine;

public abstract class FEnemyMovementBase : MonoBehaviour, IInitializable
{
    public int SideDirection { get; protected set; }
    public abstract void Initialize();
    public abstract void Play();
    public abstract void TriggerAttack();
    public abstract void TriggerFall();
}
