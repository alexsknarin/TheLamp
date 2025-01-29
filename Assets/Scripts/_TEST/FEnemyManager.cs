using System;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private FMothling _mothlingEnemy;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;

    private MothlingMovementStateFactory _mothlingMovementStateFactory;

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
        if (Input.GetKeyDown(KeyCode.A))
        {
            _lampCollisionDetectionService.AddCollidable(_mothlingEnemy);
            
            if (_mothlingEnemy.IsReadyToAttack)
            {
                _mothlingEnemy.Attack();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            _lampCollisionDetectionService.RemoveCollidable(_mothlingEnemy);
            _mothlingEnemy.DoDeath();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            _lampCollisionDetectionService.RemoveCollidable(_mothlingEnemy);
            _mothlingEnemy.Spread();
        }
    }
}
