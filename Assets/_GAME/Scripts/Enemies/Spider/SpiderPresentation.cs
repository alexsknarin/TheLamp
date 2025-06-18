using System;
using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Spider
{
    public class SpiderPresentation: MonoBehaviour
    {
        [SerializeField] private Spider _spider;
        [SerializeField] private SpiderMovement _movement;
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private DamageFlash _damageFlash;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private HealthIndication _healthIndication;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private SpiderWebController _spiderWeb;

        [SerializeField] private Animator _animator;
        private bool _isEnterEnded;
    
        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _healthIndication.Initialize();
            _spiderWeb.Initialize();
            _isEnterEnded = false;
            
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.AttackEnded += OnAttackEnded;
            _movement.SpreadStateStarted += OnSpreadStateStarted;
            _movement.SpreadStateEnded += OnSpreadStateEnded;
            
            _spider.Started += OnSpiderStarted;
            _spider.Damaged += OnSpiderDamaged;
            _spider.HealthChanged += _healthIndication.Refresh;
            _spider.Dead += OnSpiderDead;
        
        }
    
        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.AttackEnded -= OnAttackEnded;
            _movement.SpreadStateStarted -= OnSpreadStateStarted;
            _movement.SpreadStateEnded -= OnSpreadStateEnded;
            _spider.Started -= OnSpiderStarted;
            _spider.Damaged -= OnSpiderDamaged;
            _spider.HealthChanged -= _healthIndication.Refresh;
            _spider.Dead -= OnSpiderDead;
        }

        private void OnSpreadStateStarted()
        {
            _animator.SetTrigger("Spread");
        }

        private void OnSpreadStateEnded()
        {
            _animator.SetTrigger("EnterStart");
            _isEnterEnded = false;
        }

        private void Update()
        {
            if (!_isEnterEnded)
            {
                if (transform.position.y < 1.0f)
                {
                    _isEnterEnded = true;
                    _animator.SetTrigger("EnterEnd");
                }
            }
        }

        private void OnSpiderStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
            _spiderWeb.Play();
            _isEnterEnded = false;
            _animator.SetTrigger("EnterStart");
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();

            if (transform.position.x > 0)
            {
                _animator.SetTrigger("PreAttackR");                
            }
            else
            {
                _animator.SetTrigger("PreAttackL");
            }
            
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
            
            if (transform.position.x > 0)
            {
                _animator.SetTrigger("AttackR");                
            }
            else
            {
                _animator.SetTrigger("AttackL");
            }
        }

        private void OnAttackEnded()
        {
            _animator.SetTrigger("AttackEnd");
        }

        private void OnSpiderDamaged()
        {
            _damageFlash.Play();
        }

        private void OnSpiderDead()
        {
            _deathFlash.Play();
            _spiderWeb.StartShrink(true);
        }
    }
}
