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
            Debug.Log("Dragonfly entered attack zone");
            // other.attachedRigidbody.gameObject.GetComponent<DragonflyCollisionController>().SoloCollider(other);
            other.attachedRigidbody.gameObject.GetComponent<EnemyBase>().HandleEnteringAttackZone(other);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        // if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("StickyEnemy"))
        // {
        //     var enemy = other.GetComponent<EnemyBase>();
        //     enemy.HandleExitingAttackExitZone();
        // }
        // if (other.gameObject.CompareTag("Boss"))
        // {
        //     other.GetComponent<BossBase>().HandleExitingAttackExitZone();
        // }
        // if (other.gameObject.CompareTag("Dragonfly"))
        // {
        //     other.attachedRigidbody.gameObject.GetComponent<EnemyBase>().HandleExitingAttackExitZone();
        // }
    }
}
