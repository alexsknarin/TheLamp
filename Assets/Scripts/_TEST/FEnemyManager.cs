using System;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private FMothling _mothlingEnemy;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _lampCollisionDetectionService.AddCollidable(_mothlingEnemy);
            _mothlingEnemy.Attack();
        }
    }
}
