using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private DragonflyDamageFlash _damageIndication;
        [SerializeField] private DragonflyHealthIndication _healthIndication;
        [SerializeField] private DragonflyDeathFlash _deathFlash;
        [SerializeField] private DragonflyPreAttackFlash _preAttackFlash;
        [FormerlySerializedAs("_swarmCallPresentation")] [SerializeField] private DragonflySwarmCallFX _swarmCallFX;

        public void PreAttackStart()
        {
            _preAttackFlash.PreAttackStart();
        }

        public void PreAttackEnd()
        {
            _preAttackFlash.PreAttackEnd();
        }
    
        public void SetActiveColliderTransform(Transform transform)
        {
            _damageIndication.SetContactCollisionTransform(transform);
        }
    
        public void DamageFlash()
        {
            _damageIndication.Play();
        }

        public void DeathFlash()
        {
            _deathFlash.Play();
        }

        public void HealthUpdate(int currentHealth, int maxHealth)
        {
            _healthIndication.Refresh(currentHealth, maxHealth);
        }

        public void SwarmCall()
        {
            _swarmCallFX.Play();
        }

        public void Initialize()
        {
            _damageIndication.Initialize();
            _healthIndication.Initialize();
            _deathFlash.Initialize();
            _preAttackFlash.Initialize();
            _swarmCallFX.Initialize();
        }
    
    
    }
}
