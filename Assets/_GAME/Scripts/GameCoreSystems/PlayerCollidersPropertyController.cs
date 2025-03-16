using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems
{
    public class PlayerCollidersPropertyController : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _attackZoneCollider;
        [SerializeField] private Transform _attackZonePresentationTransform;
        [SerializeField] private CircleCollider2D _attackExitZoneCollider;
    
        public void SetAttackZoneRadius(float radius)
        {
            _attackZoneCollider.radius = radius;
            Vector3 scale = Vector3.one * (radius/0.5f);
            _attackZonePresentationTransform.localScale = scale;
        }
    }
}
