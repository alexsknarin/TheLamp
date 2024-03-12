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
}
