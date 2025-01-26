using System;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private FMothling _mothlingEnemy;
    [SerializeField] private FMothlingRef _mothlingRefEnemy;
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
        Debug.Log("FEnemyManager Awake - enemy constructed");
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
            _lampCollisionDetectionService.AddCollidable(_mothlingRefEnemy);
            if (_mothlingEnemy.IsReadyToAttack)
            {
                _mothlingEnemy.Attack();
            }
            _mothlingRefEnemy.Attack();
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            _lampCollisionDetectionService.RemoveCollidable(_mothlingEnemy);
            _mothlingEnemy.HandleDeath();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            _lampCollisionDetectionService.RemoveCollidable(_mothlingEnemy);
            _mothlingEnemy.Spread();
        }
    }
}
