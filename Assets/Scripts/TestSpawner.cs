using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    
    private void Start()
    {
        _enemyPrefab.Initialize();
    }
}
