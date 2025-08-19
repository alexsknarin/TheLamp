using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileSpider
{
    public class DragonflySpiderPresentation : MonoBehaviour, IInitializable
    {
        private static readonly int EnterStart = Animator.StringToHash("EnterStart");
        private static readonly int ToCurl = Animator.StringToHash("ToCurl");
        [SerializeField] private DeathFlash _deathFlash; 
        [Header("------ Preattack Flash ------")]
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private DragonflySpiderWebController _spiderWeb;
        [SerializeField] private DragonflyProjectileSpiderBodyRotationHandler _spiderBodyRotationHandler;
        [SerializeField] private Animator _animator;
    
        public void Initialize()
        {
            _deathFlash.Initialize();
            _preAttackFlash.Initialize();
            _spiderWeb.Initialize();
            _spiderBodyRotationHandler.Initialize();
        }
    
        public void Play()
        {
            _spiderWeb.Play(transform);
            _deathFlash.Initialize();
            _animator.SetTrigger(EnterStart);
        }
    
        public void SwitchToCaughtState()
        {
            _spiderWeb.StartShrink();
            _animator.SetTrigger(ToCurl);
        }
    
        public void PreAttackStart()
        {
            _preAttackFlash.PreAttackStart();
        }

        public void PreAttackEnd()
        {
            _preAttackFlash.PreAttackEnd();
        }

        public void DeathFlash()
        {
            _deathFlash.Play();
        }
    }
}
