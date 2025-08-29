using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaSpiderSwarm : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _projectilePosition01;
        [SerializeField] private Transform _projectilePosition02;
        [SerializeField] private MegaSpiderProjectileSpider.MegaSpiderProjectileSpider _projectile01;
        [SerializeField] private MegaSpiderProjectileSpider.MegaSpiderProjectileSpider _projectile02;
        [SerializeField] private Transform _projectile01Transform;
        [SerializeField] private Transform _projectile02Transform;
    
        public void Initialize()
        {
            _projectile01.Initialize();
            _projectile02.Initialize();
        
            Reset();
        }

        public void Reset()
        {
            // TODO: potentially disable or do something else with swarm when in non swarm attack state
            Debug.Log("MegaSpiderSwarm Reset");
            _projectile01Transform.SetParent(_projectilePosition01);
            _projectile01Transform.localPosition = Vector3.zero;
            _projectile01.gameObject.SetActive(true);
            _projectile01.Play();
            _projectile02Transform.SetParent(_projectilePosition02);
            _projectile02Transform.localPosition = Vector3.zero;
            _projectile02.gameObject.SetActive(true);
            _projectile02.Play();
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
