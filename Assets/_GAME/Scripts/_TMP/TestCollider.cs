using _GAME.Scripts.Enemies.MegaSpider;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    [SerializeField] private MegaSpider _megaSpider;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private float _collisionDistance;

    private bool _isEnteredAttackZone;
    
    
    void Update()
    {
        if (_visibleBodyTransform.position.magnitude < _collisionDistance+0.1f && !_isEnteredAttackZone)
        {
            _isEnteredAttackZone = true;
        }

        if (_visibleBodyTransform.position.magnitude > _collisionDistance + 0.1f && _isEnteredAttackZone)
        {
            _megaSpider.AttackZoneExit();
            _isEnteredAttackZone = false;
        }
        
        if (_visibleBodyTransform.position.magnitude < _collisionDistance)
        {
            _megaSpider.Collide();
        }
        
    }
}
