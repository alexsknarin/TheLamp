using System;
using System.Collections.Generic;

public class FLampAttacker
{
    private bool _isBlockedMode = false;
    
    public void SetBlockedMode()
    {
        _isBlockedMode = true;
    }
    
    public void SetUnBlockedMode()
    {
        _isBlockedMode = false;
    }
    
    public void Attack(float power, List<FEnemy> enemies)
    {
        foreach (var enemy in enemies)
        {
            if (_isBlockedMode)
            {
                if(enemy is IStickableWithLamp && enemy.IsReadyForDamage)
                {
                    int damage = Converters.PowerToAttackPower(power);
                    if (damage > 0)
                    {
                        enemy.ReceiveDamage(damage);
                    }
                }    
            }
            else
            {
                if(enemy.IsReadyForDamage)
                {
                    int damage = Converters.PowerToAttackPower(power);
                    if (damage > 0)
                    {
                        enemy.ReceiveDamage(damage);
                    }
                }
            }
            
        }
    }
}
