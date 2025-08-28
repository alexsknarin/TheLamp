using _GAME.Scripts.Enemies.MegaSpider;
using _GAME.Scripts.Enemies.MegaSpiderProjectileSpider;
using _GAME.Scripts.Enemies.MegaSpiderProjectileSpider.MovementStates;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    [SerializeField] private MegaSpider _megaSpider;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private float _collisionDistance;
    [Header("Projectile01")]
    [SerializeField] private float _projectileCollisionDistance;
    [SerializeField] private MegaSpiderProjectileSpider _projectileSpider01;
    [SerializeField] private Transform _projectile01Transform;
    // [Header("Projectile02")]
    // [SerializeField] private MegaSpiderProjectileSpider _projectileSpider02;
    // [SerializeField] private Transform _projectile02;

    private bool _isEnteredAttackZone;
    private bool _isP01EnteredAttackZone;
    
    
    void Update()
    {
        // Main Spider
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
        
        // Projectile01
        if (_projectile01Transform.position.magnitude < _projectileCollisionDistance+0.1f && !_isP01EnteredAttackZone)
        {
            _isP01EnteredAttackZone = true;
        }
        
        if (_projectile01Transform.position.magnitude > _projectileCollisionDistance + 0.1f && _isP01EnteredAttackZone)
        {
            _projectileSpider01.AttackZoneExit();
            _isP01EnteredAttackZone = false;
        }
        
        if (_projectile01Transform.position.magnitude < _projectileCollisionDistance)
        {
            _projectileSpider01.Collide();
        }
    }
}
