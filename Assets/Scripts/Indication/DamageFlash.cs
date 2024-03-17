using UnityEngine;
using UnityEngine.VFX;

public class DamageFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private VisualEffect _damageParticles;
    private Material _material;
    private bool _isActive = false;
    private float _localTime;
    
    public override void Play()
    {
        _isActive = true;
        _localTime = 0;
        Vector3 direction = transform.position.normalized;
        _damageParticles.SetVector3("Direction", direction);
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
                _material.SetFloat("_AttackSemaphore", 0f);
                _material.SetFloat("_Damage", 1f);
                return;
            }
            _material.SetFloat("_AttackSemaphore", 1-phase);
            _material.SetFloat("_Damage", 9f);
            _localTime += Time.deltaTime;
        }
    }
    
    public override void Initialize()
    {
        _material = _meshRenderer.material;
        _material.SetFloat("_AttackSemaphore", 0f);
        _material.SetFloat("_Damage", 1f);
    }
}
