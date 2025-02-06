using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    // TODO: move this to the Game Context Root
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private FMothling _mothlingEnemy;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;
    private FEnemy _enemy;

    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    private FlyMovementStateFactory _flyMovementStateFactory;
    private FEnemyFactory _enemyFactory;
    private FEnemyPool _enemyPool;
    
    private List<IDamageable> _damageables = new();

    private void Awake()
    {
        _mothlingMovementStateFactory = new MothlingMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _flyMovementStateFactory = new FlyMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        
        _enemyFactory = new FEnemyFactory(_mothlingMovementStateFactory, _flyMovementStateFactory);
        _enemyPool = new FEnemyPool(_enemyFactory);
        _enemyPool.Initialize();
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _enemyPool.PreloadEnemy(typeof(FFireFly));
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            _enemy = _enemyPool.Get(typeof(FFireFly));
            _enemy.Play();
        }

        // Start Enemy Attack
        if (Input.GetKeyDown(KeyCode.A))
        {
            _lampCollisionDetectionService.AddCollidable(_enemy);
            _damageables.Add(_enemy);
           
            if (_enemy.IsReadyToAttack)
            {
                _enemy.Attack();
            }
        }
        
        // Emulate enemy Death
        if (Input.GetKeyDown(KeyCode.D))
        {
            _lampCollisionDetectionService.RemoveCollidable(_enemy);
            _enemy.DoDeath();
        }
        
        // Emulate enemy Spread
        if (Input.GetKeyDown(KeyCode.S))
        {
            _lampCollisionDetectionService.RemoveCollidable(_enemy);
            _enemy.Spread();
        }
        
        // Do Lamp Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (_damageables.Count != 0)
            {
                foreach (var damageable in _damageables)
                {
                    if (damageable.IsReadyForDamage)
                    {
                        damageable.ReceiveDamage(3);
                    }
                }
                
                _damageables.Clear();
            }
        }
        
    }
}
