using System;
using _GAME.Scripts.Enemies.MegaSpiderProjectileSpider;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

public class MegaSpiderSwarm : MonoBehaviour, IInitializable
{
    [SerializeField] private Transform _projectilePosition01;
    [SerializeField] private Transform _projectilePosition02;
    [SerializeField] private MegaSpiderProjectileSpider _projectile01;
    
    
    public void Initialize()
    {
        _projectile01.Initialize();
    }

    // TODO: TMP
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P");
            _projectile01.Play();
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            _projectile01.Attack();
        }
    }
}
