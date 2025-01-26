using UnityEngine;

public abstract class FEnemyMovementBase : MonoBehaviour, IInitializable
{
    public int SideDirection { get; protected set; }
    public abstract void Initialize();
    public abstract void Play(); // IEnemyMovement
    public abstract void TriggerAttack(); // IEnemyMovement
    public abstract void TriggerFall(); // IEnemyMovement
    public abstract void TriggerDeath(); // IEnemyMovement
    
}
