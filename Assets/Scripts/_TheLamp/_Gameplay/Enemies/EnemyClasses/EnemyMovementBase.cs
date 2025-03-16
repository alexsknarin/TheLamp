using UnityEngine;

public abstract class EnemyMovementBase : MonoBehaviour, IInitializable, IEnemyMovable
{
    protected bool _isAttacking = false;
    public virtual int SideDirection { get; protected set; }
    public abstract void Initialize();
    public abstract void Play();
    public abstract void TriggerAttack();
    public abstract void TriggerFall(); 
    public abstract void TriggerDeath(); 
}
