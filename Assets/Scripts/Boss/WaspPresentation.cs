using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaspPresentation : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _waspBodyMeshRenderer;
    [SerializeField] private TrailResetHandler _trailResetHandler;
    [SerializeField] private VisualEffect _damageParticles;
    [SerializeField] private VisualEffect _damageEmitParticles;
    [SerializeField] private float _deathDuration;
    
    private bool _isDead;
    private bool _isDamaged;
    private Material _waspBodyMaterial;
    private WaitForSeconds _damageFlashDuration = new WaitForSeconds(0.8f);
    private float _localTime;

    private IEnumerator WaitForDamageFlashEnd()
    {
        yield return _damageFlashDuration;
        _waspBodyMaterial.SetInt("_isDamaged", 0);
    }
    
    public void Initialize()
    {
        _localTime = 0;
        _isDead = false;
        _waspBodyMaterial = _waspBodyMeshRenderer.material;
        _waspBodyMaterial.SetFloat("_DamagePhase", 0);
        _waspBodyMaterial.SetInt("_isDamaged", 0);
        _waspBodyMaterial.SetFloat("_DeathPhase", 0);
        
        _damageEmitParticles.SendEvent("OnEndEmit");
        _damageEmitParticles.SetFloat("Rate", 0);
    }
    
    public void SetDamage(float damage)
    {
        _waspBodyMaterial.SetFloat("_DamagePhase", damage);
        _waspBodyMaterial.SetInt("_isDamaged", 1);
        
        float emitRate = damage * 22;
        _damageEmitParticles.SendEvent("OnStartEmit");
        _damageEmitParticles.SetFloat("Rate", emitRate);
        StartCoroutine(WaitForDamageFlashEnd());
    }
    
    public void PlayDeath()
    {
        _isDead = true;
        _localTime = 0;
        _damageEmitParticles.SendEvent("OnStartEmit");
        _damageEmitParticles.SetFloat("Rate", 35);
    }
    
    public void ResetTrail()
    {
        _trailResetHandler.Initialize();
    }

    private void PerformDeath()
    {
        if (_isDead)
        {
            float phase = _localTime / _deathDuration;
            _waspBodyMaterial.SetFloat("_DeathPhase", phase);
            
            if (phase > 1)
            {
                _isDead = false;
                _waspBodyMaterial.SetFloat("_DeathPhase", 1);
                _damageEmitParticles.SendEvent("OnEndEmit");
                _damageEmitParticles.SetFloat("Rate", 0);
                _trailResetHandler.Initialize();
                return;
            }
            
            _localTime += Time.deltaTime;
        }
    }
    
    public void PlayDamageParticles()
    {
        Vector3 direction = transform.position;
        direction.z = 0;
        direction.Normalize();
        _damageParticles.SetVector3("Direction", direction);
        _damageParticles.SendEvent("OnDamage");
    }
    
    private void Update()
    {
        PerformDeath();
    }
}
