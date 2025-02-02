using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private async void Awake()
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
        _enemyFactory = new FEnemyFactory(_mothlingMovementStateFactory);
        
        
    }
    
    private async void LoadEnemy()
    {
        // await Task.Delay(10000);
        _enemy = await _enemyFactory.CreateMothling();
        Debug.Log("Enemy Loaded +++++++++++++++++++");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadEnemy();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_enemy != null)
            {
                _enemy.Play();
            }
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
