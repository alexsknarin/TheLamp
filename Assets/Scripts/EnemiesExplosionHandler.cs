using System.Collections.Generic;
using UnityEngine;

public class EnemiesExplosionHandler
{
    public void HandleExplosion(List<EnemyBase> enemies, EnemyBase explosionSource, Vector3 explosionPosition, float fireflyExplosionRadius)
    {
        foreach (var enemy in enemies)
        {
            if (enemy == explosionSource)
            {
                continue;
            }
            Vector3 enemyPosition2d = enemy.transform.position;
            enemyPosition2d.z = 0;
            Vector3 explosionPosition2d = explosionPosition;
            explosionPosition2d.z = 0;
            if((explosionPosition2d - enemyPosition2d).magnitude < fireflyExplosionRadius)
            {
                enemy.ReceiveDamage(100);
            }
        }
    }
}
