
using UnityEngine;

public class LampClosestColliderCatcherZoneHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dragonfly"))
        {
            other.attachedRigidbody.gameObject.GetComponent<Dragonfly>().CatchFirstCollider();
        }
    }
}
