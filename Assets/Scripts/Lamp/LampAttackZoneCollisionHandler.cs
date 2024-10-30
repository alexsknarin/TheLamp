using UnityEngine;

public class LampAttackZoneCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") 
            || other.gameObject.CompareTag("StickyEnemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            enemy.HandleEnteringAttackZone(other);
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            other.GetComponent<BossBase>().HandleEnteringAttackZone(other);
        }
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            other.attachedRigidbody.gameObject.GetComponent<EnemyBase>().HandleEnteringAttackZone(other);
        }
    }
}
