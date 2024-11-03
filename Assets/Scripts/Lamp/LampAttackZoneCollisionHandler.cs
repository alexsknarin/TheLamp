using UnityEngine;

public class LampAttackZoneCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("StickyEnemy"))
        {
            other.GetComponent<EnemyBase>().HandleEnteringAttackZone();
        }
        
        if (other.gameObject.CompareTag("Boss"))
        {
            other.GetComponent<BossBase>().HandleEnteringAttackZone();
        }
        
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            other.attachedRigidbody.gameObject.GetComponent<Dragonfly>().HandleEnteringAttackZone();
        }
    }
}
