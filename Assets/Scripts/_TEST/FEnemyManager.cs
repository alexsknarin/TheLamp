using System;
using System.Collections.Generic;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private FMothling _mothlingEnemy;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;
    private FEnemy _enemy;

    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    private FEnemyFactory _enemyFactory;
    
    private List<IDamageable> _damageables = new();

    private void Awake()
    {
        // _mothlingMovementStateFactory = new MothlingMovementStateFactory(
        //     _cameraTransform,
        //     _lampPositionProviderService
        //     );
        // _mothlingEnemy.GetComponent<FMothlingMovement>().Construct(_mothlingMovementStateFactory);
        // _mothlingEnemy.GetComponent<FMothlingPresentation>().Initialize();
        // _mothlingEnemy.Initialize();
        
        _mothlingMovementStateFactory = new MothlingMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _enemyFactory = new FEnemyFactory(_mothlingEnemy, _mothlingMovementStateFactory);
        
        _enemy = _enemyFactory.CreateMothling();
    }

    private void Start()
    {
        _enemy.Play();
    }

    private void Update()
    {
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
