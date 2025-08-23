using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaSpider : MonoBehaviour
    {
        [SerializeField] private string _stateDebug;
        [Header("-- Attributes --")]
        [SerializeField] private int _maxHealth = 24;
        [SerializeField] private int _currentHealth;
        [Header("-- Movement --")]
        [SerializeField] private MegaSpiderMovement _movement;

        private void Awake()
        {
            Initialize();
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Play();
            }
        }

        public void Initialize()
        {
            _movement.Initialize();
        }
        
        public void Play()
        {
            _movement.Play();
        }
    }
}
