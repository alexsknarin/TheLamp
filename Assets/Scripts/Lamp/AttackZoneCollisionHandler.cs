using UnityEngine;

public class AttackZoneCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") 
            || other.gameObject.CompareTag("StickyEnemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.HandleEnteringAttackZone();
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            other.GetComponent<Wasp>().HandleEnteringAttackZone();
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
            other.GetComponent<Wasp>().HandleExitingAttackExitZone();
        }
        
    }
}
