using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpiderProjectileSpider
{
    public class MegaspiderProjectilePresentation : MonoBehaviour, IInitializable
    {
        private static readonly int ToIdle = Animator.StringToHash("ToIdle");
        private static readonly int ToJump = Animator.StringToHash("ToJump");
        private static readonly int ToFall = Animator.StringToHash("ToFall");
        private static readonly int AttackSemaphore = Shader.PropertyToID("_AttackSemaphore");
        private static readonly int DeathFade = Shader.PropertyToID("_DeathFade");
        [SerializeField] private MegaspiderProjectileSpider.MegaspiderProjectileSpider _projectile;
        [SerializeField] private Animator _animator;
        [SerializeField] private MeshRenderer[] _meshRenderers;
        [SerializeField] private float _preAttackDuration = 0.2f;
        [SerializeField] private float _deathFadeDuration = 1.2f;
        private List<Material> _materials = new ();
        private WaitForSeconds _waitForPreAttackEnd;
        private float _localTime;


        public void Initialize()
        {
            _projectile.Started += OnProjectileStarted;
            _projectile.JumpStarted += OnProjectileJumpStarted;
            _projectile.FallStarted += OnProjectileFallStarted;
            _projectile.Damaged += OnProjectileDamaged;
            
            foreach (var meshRenderer in _meshRenderers)
            {
                _materials.Add(meshRenderer.material);
            }

            _waitForPreAttackEnd = new(_preAttackDuration);
        }

        private void OnDestroy()
        {
            _projectile.Started -= OnProjectileStarted;
            _projectile.JumpStarted -= OnProjectileJumpStarted;
            _projectile.FallStarted -= OnProjectileFallStarted;
            _projectile.Damaged -= OnProjectileDamaged;
        }

        private void OnProjectileStarted()
        {
            _animator.SetTrigger(ToIdle);
            foreach (var material in _materials)
            {
                material.SetFloat(DeathFade, 0);
            }
        }

        private void OnProjectileJumpStarted()
        {
            _animator.SetTrigger(ToJump);
            foreach (var material in _materials)
            {
                material.SetFloat(AttackSemaphore, 1);
            }
            StartCoroutine(WaitForPreAttackEnd());
        }

        private IEnumerator WaitForPreAttackEnd()
        {
            yield return _waitForPreAttackEnd;
            foreach (var material in _materials)
            {
                material.SetFloat(AttackSemaphore, 0);
            }
        }

        private void OnProjectileFallStarted()
        {
            _animator.SetTrigger(ToFall);
            
        }

        private void OnProjectileDamaged()
        {
            _localTime = 0;
            StartCoroutine(PerformDeathFade());
        }

        private IEnumerator PerformDeathFade()
        {
            while (_localTime < _deathFadeDuration)
            {
                _localTime += Time.deltaTime;
                yield return null;
                float phase = _localTime / 1.2f;
                foreach (var material in _materials)
                {
                    material.SetFloat(DeathFade, phase);
                }                
            }
        }
    }
}
