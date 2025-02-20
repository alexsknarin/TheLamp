using System.Collections.Generic;

public class FLampAttacker
{
    public void Attack(float power, List<FEnemy> enemies)
    {
        foreach (var enemy in enemies)
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
