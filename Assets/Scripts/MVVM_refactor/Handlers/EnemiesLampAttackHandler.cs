using System;
using System.Collections.Generic;

public class EnemiesLampAttackHandler
{
    public event Action OnEnemyDamagedEvent;
   
    public void HandleLampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance,
        List<EnemyBase> enemies)
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
    
    public void HandleLampBlockedAttack(int attackPower, float currentPower, float attackDuration, float attackDistance,
        List<EnemyBase> enemies)
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
