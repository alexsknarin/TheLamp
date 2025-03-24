using System.Collections.Generic;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;

namespace _GAME.Scripts.GameCoreSystems.EnemyManagement
{
    public class LampAttacker
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
}
