using UnityEngine;

public class AttackExitZoneCollisionHandler : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.HandleExitingAttackExitZone();
        }
    }
}
