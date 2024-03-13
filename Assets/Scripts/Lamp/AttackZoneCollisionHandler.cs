using UnityEngine;

public class AttackZoneCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("StickyEnemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.HandleEnteringAttackZone();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.HandleExitingAttackExitZone();
        }
    }
}
