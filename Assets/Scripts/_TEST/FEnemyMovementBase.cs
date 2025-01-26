using UnityEngine;

public abstract class FEnemyMovementBase : MonoBehaviour, IInitializable, IEnemyMovable, ISpreadable
{
    public int SideDirection { get; protected set; }
    public abstract void Initialize();
    public abstract void Play();
    public abstract void TriggerAttack();
    public abstract void TriggerFall(Vector2 newPosition); 
    public abstract void TriggerDeath(); 
    public abstract void TriggerSpread();
}
