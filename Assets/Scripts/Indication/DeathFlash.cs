using UnityEngine;
using UnityEngine.VFX;

public class DeathFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private VisualEffect _deathParticles;
    [SerializeField] private VisualEffect _damageParticles;
    private Material _material;
    private bool _isActive = false;
    private float _localTime;
    
    public override void Play()
    {
        _isActive = true;
        _localTime = 0;
        _material.SetFloat("_DeathFade", 0);
        _material.SetFloat("_AttackSemaphore", 0);
        _material.SetFloat("_Damage", 1f);
        Vector3 direction = transform.position.normalized;
        _deathParticles.gameObject.SetActive(true);
        _damageParticles.SetVector3("Direction", direction);
        _deathParticles.SendEvent("OnDeathStart");
        _damageParticles.SendEvent("OnDamage");
    }

    void Update()
    {
        if (_isActive)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isActive = false;
                _material.SetFloat("_DeathFade", 1f);
                _material.SetFloat("_Damage", 1f);
                _deathParticles.SendEvent("OnDeathStart");
                // _deathParticles.gameObject.SetActive(false);
                return;
            }
            _material.SetFloat("_Damage", 4f);
            _material.SetFloat("_DeathFade", phase);
            _localTime += Time.deltaTime;
        }
    }

    public override void Initialize()
    {
        _isActive = false;
        _material = _meshRenderer.material;
        _material.SetFloat("_DeathFade", 0f);
        _material.SetFloat("_AttackSemaphore", 0f);
        _material.SetFloat("_Damage", 1f);
        _deathParticles.gameObject.SetActive(false);
    }
}