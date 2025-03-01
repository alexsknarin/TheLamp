using UnityEngine;

public abstract class FEnemyMovementBase : MonoBehaviour, IInitializable, IEnemyMovable, ISpreadableMovement
{
    protected bool _isAttacking = false;
    public virtual int SideDirection { get; protected set; }
    public abstract void Initialize();
    public abstract void Play();
    public abstract void TriggerAttack();
    public abstract void TriggerFall(); 
    public abstract void TriggerDeath(); 
    public abstract void TriggerSpread();
}
