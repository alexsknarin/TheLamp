using System;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private FMothling _mothlingEnemy;
    [SerializeField] private FMothlingRef _mothlingRefEnemy;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _lampCollisionDetectionService.AddCollidable(_mothlingEnemy);
            _lampCollisionDetectionService.AddCollidable(_mothlingRefEnemy);
            _mothlingEnemy.Attack();
            _mothlingRefEnemy.Attack();
        }
    }
}
