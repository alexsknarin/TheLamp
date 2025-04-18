using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class DamageEmitParticles : MonoBehaviour
    {
        [SerializeField] private VisualEffect _damageEmitParticles;
        
        const float DamageRateMultiplier = 22f;
        const float DeathRateMultiplier = 35f;
        
        public void Initialize()
        {
            _damageEmitParticles.gameObject.SetActive(false);
        }

        public void HandleHealthChanged(int currentHealth, int maxHealth)
        {
            if (currentHealth == maxHealth) return;
            
            _damageEmitParticles.gameObject.SetActive(true);
            
            float damagePhase = Mathf.Clamp01(1 - (float)currentHealth/maxHealth);
            float emitRate = damagePhase * DamageRateMultiplier;
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", emitRate);
        }

        public void HandleDead()
        {
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", DeathRateMultiplier);
        }

        public void HandleDeathEnd()
        {
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", 0);
            _damageEmitParticles.gameObject.SetActive(false);
        }
    }
}
