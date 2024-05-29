using System;
using System.Collections.Generic;

public class EnemiesLampAttackHandler
{
    public event Action OnEnemyDamaged;
   
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
                    OnEnemyDamaged?.Invoke();    
                }
            }
        }
    }
    
    public void HandleLampBlockedAttack(int attackPower, float currentPower, float attackDuration, float attackDistance,
        List<EnemyBase> enemies)
    {
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy && enemy.ReadyToLampDamage && enemy.IsStick && enemy.EnemyType == EnemyTypes.Ladybug)
            {
                if (attackPower > 0)
                {
                    enemy.ReceiveDamage(attackPower);
                    OnEnemyDamaged?.Invoke();   
                }
            }
        }
    }
}
