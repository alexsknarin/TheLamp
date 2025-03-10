using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FDragonfly : MonoBehaviour
{
    [SerializeField] private FDragonflyMovement _movement;

    private void Start()
    {
        _movement.Initialize();
        StartCoroutine(StartMovement());
    }
    
    private IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(1f);
        var enterType = (DragonflyEnterType)Random.Range(0, 2);
        int sideDirection = RandomDirection.Generate();
        _movement.Play(enterType, sideDirection);
    }
}
