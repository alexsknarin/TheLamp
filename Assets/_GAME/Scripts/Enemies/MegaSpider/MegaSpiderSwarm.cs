using System;
using System.Collections;
using _GAME.Scripts.Enemies.MegaspiderProjectileSpider;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderSwarm : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _projectilePosition01;
        [SerializeField] private Transform _projectilePosition02;
        [SerializeField] private MegaspiderProjectileSpider.MegaspiderProjectileSpider _projectile01;
        [SerializeField] private MegaspiderProjectileSpider.MegaspiderProjectileSpider _projectile02;
        [SerializeField] private Transform _projectile01Transform;
        [SerializeField] private Transform _projectile02Transform;
        
        // Dependencies
        private Transform _lampTransform;

        public event Action<bool> Projecile01FallEnded; // TODO: create delegate to name bool properly
        public event Action<bool> Projecile02FallEnded;
        
        public CollidableEnemy Projectile01 => _projectile01;
        public CollidableEnemy Projectile02 => _projectile02;
        
        
        public void Construct(Transform lampTransform)
        {
            _lampTransform = lampTransform;
            _projectile01.GetComponent<MegaspiderProjectileSpiderMovement>().Construct(_lampTransform);
            _projectile02.GetComponent<MegaspiderProjectileSpiderMovement>().Construct(_lampTransform);
        } 
        
        public void Initialize()
        {
            _projectile01.Initialize();
            _projectile02.Initialize();

            _projectile01.FallEnded += OnProjectile01FallEnded;
            _projectile02.FallEnded += OnProjectile02FallEnded;
            
            
            
            Reset();
        }

        private void OnProjectile01FallEnded()
        {
            Projecile01FallEnded?.Invoke(_projectile01.IsReceivedLampAttackDamage);
        }

        private void OnProjectile02FallEnded()
        {
            Projecile02FallEnded?.Invoke(_projectile02.IsReceivedLampAttackDamage);
        }

        private void OnDestroy()
        {
            _projectile01.FallEnded -= OnProjectile01FallEnded;
            _projectile02.FallEnded -= OnProjectile02FallEnded;
        }

        public void Reset()
        {
            if (_projectile01.IsAttackStarted)
            {
                Debug.LogError("Projectile01 Force Fall");
                Debug.Break();
            }
            
            _projectile01Transform.SetParent(_projectilePosition01);
            _projectile01Transform.localPosition = Vector3.zero;
            _projectile01.gameObject.SetActive(true);
            _projectile01.Play();

            if (_projectile02.IsAttackStarted)
            {
                Debug.LogError("Projectile02 Force Fall");
                Debug.Break();
            }
            
            _projectile02Transform.SetParent(_projectilePosition02);
            _projectile02Transform.localPosition = Vector3.zero;
            _projectile02.gameObject.SetActive(true);
            _projectile02.Play();
        }

        public void HideProjectiles()
        {
            _projectile01.gameObject.SetActive(false);
            _projectile02.gameObject.SetActive(false);
        }
        
        public void Attack01()
        {
            _projectile01.Attack();
        }
    
        public void Attack02()
        {
            _projectile02.Attack();
        }
    }
}
