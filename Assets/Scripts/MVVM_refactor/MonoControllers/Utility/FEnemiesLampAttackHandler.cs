using System;
using System.Collections.Generic;

public class FEnemiesLampAttackHandler
{
    public event Action OnEnemyDamagedEvent; // TODO: find out if we need this event
   
    public void HandleLampAttack(List<EnemyBase> enemies, int attackPower)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy && enemy.ReadyToLampDamage)
            {
                if (attackPower > 0)
                {
                    enemy.ReceiveDamage(attackPower);
                    OnEnemyDamagedEvent?.Invoke();
                }
            }
        }
    }
    
    public void HandleLampBlockedAttack(List<EnemyBase> enemies, int attackPower)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy && 
                enemy.ReadyToLampDamage && enemy.IsStick && 
                (enemy.EnemyType == EnemyType.Ladybug || enemy.EnemyType == EnemyType.Megabeetle))
            {
                if (attackPower > 0)
                {
                    enemy.ReceiveDamage(attackPower);
                    OnEnemyDamagedEvent?.Invoke();   
                }
            }
        }
    }
}
