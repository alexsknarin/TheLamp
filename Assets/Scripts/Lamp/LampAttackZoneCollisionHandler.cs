using UnityEngine;

public class LampAttackZoneCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") 
            || other.gameObject.CompareTag("StickyEnemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.HandleEnteringAttackZone();
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            other.GetComponent<BossBase>().HandleEnteringAttackZone();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.HandleExitingAttackExitZone();
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            other.GetComponent<BossBase>().HandleExitingAttackExitZone();
        }
        
    }
}
