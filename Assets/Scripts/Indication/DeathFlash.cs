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
    private float _prevTime;
    
    public override void Play()
    {
        _isActive = true;
        _prevTime = Time.time;
        _material.SetFloat("_DeathFade", 0);
        _material.SetFloat("_AttackSemaphore", 0);
        _material.SetFloat("_Damage", 1f);
        Vector3 direction = transform.position.normalized;
        _damageParticles.SetVector3("Direction", direction);
        _deathParticles.SendEvent("OnDeathStart");
        _damageParticles.SendEvent("OnDamage");
    }

    void Update()
    {
        if (_isActive)
        {
            float phase = (Time.time - _prevTime) / _duration;
            if (phase > 1)
            {
                _isActive = false;
                _material.SetFloat("_DeathFade", 1f);
                _material.SetFloat("_Damage", 1f);
                _deathParticles.SendEvent("OnDeathStart");
                return;
            }
            _material.SetFloat("_Damage", 4f);
            _material.SetFloat("_DeathFade", phase);
        }
    }

    public override void Initialize()
    {
        _isActive = false;
        _material = _meshRenderer.material;
        _material.SetFloat("_DeathFade", 0f);
        _material.SetFloat("_AttackSemaphore", 0f);
        _material.SetFloat("_Damage", 1f);
    }
}