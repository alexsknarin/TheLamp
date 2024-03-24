using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspPresentation : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _waspBodyMeshRenderer;
    [SerializeField] private float _deathDuration;
    private bool _isDead;
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
    }
    
    public void SetDamage(float damage)
    {
        _waspBodyMaterial.SetFloat("_DamagePhase", damage);
        _waspBodyMaterial.SetInt("_isDamaged", 1);
        StartCoroutine(WaitForDamageFlashEnd());
    }
    
    public void PlayDeath()
    {
        _isDead = true;
        _localTime = 0;
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
                return;
            }
            
            _localTime += Time.deltaTime;
        }
    }
    
    private void Update()
    {
        PerformDeath();
    }
}
