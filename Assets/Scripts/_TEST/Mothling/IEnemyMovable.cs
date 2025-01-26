using UnityEngine;

public interface IEnemyMovable
{
    public void Play(); 
    public void TriggerAttack(); 
    public void TriggerFall(Vector2 newPosition); 
    public void TriggerDeath();
}
