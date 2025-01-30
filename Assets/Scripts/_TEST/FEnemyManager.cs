using System;
using System.Collections.Generic;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private FMothling _mothlingEnemy;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;

    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    
    private List<IDamageable> _damageables = new();

    private void Awake()
    {
        _mothlingMovementStateFactory = new MothlingMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
            );
        _mothlingEnemy.GetComponent<FMothlingMovement>().Construct(_mothlingMovementStateFactory);
        _mothlingEnemy.GetComponent<FMothlingPresentation>().Initialize();
        _mothlingEnemy.Initialize();
        
        
    }

    private void Start()
    {
        _mothlingEnemy.Play();
    }

    private void Update()
    {
        // Start Enemy Attack
        if (Input.GetKeyDown(KeyCode.A))
        {
            _lampCollisionDetectionService.AddCollidable(_mothlingEnemy);
            _damageables.Add(_mothlingEnemy);
           
            if (_mothlingEnemy.IsReadyToAttack)
            {
                _mothlingEnemy.Attack();
            }
        }
        
        // Emulate enemy Death
        if (Input.GetKeyDown(KeyCode.D))
        {
            _lampCollisionDetectionService.RemoveCollidable(_mothlingEnemy);
            _mothlingEnemy.DoDeath();
        }
        
        // Emulate enemy Spread
        if (Input.GetKeyDown(KeyCode.S))
        {
            _lampCollisionDetectionService.RemoveCollidable(_mothlingEnemy);
            _mothlingEnemy.Spread();
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
