using UnityEngine;
using UnityEngine.VFX;

public class LadybugDamageFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshRenderer _attackZone;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private VisualEffect _damageParticles;
    private Material _bodyMaterial;
    private Material _attackZoneMaterial;
    private float _localTime;

    public override void Initialize()
    {
        enabled = false;
        _bodyMaterial = _meshRenderer.material;
        _attackZoneMaterial = _attackZone.material;
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _bodyMaterial.SetFloat("_Damage", 1f);
        _attackZoneMaterial.SetFloat("_Alpha", 0f);
    }

    public override void Play()
    {
        enabled = true;
        _localTime = 0;
        
        Vector3 direction = transform.position.normalized;
        _damageParticles.SetVector3("Direction", direction);
        _damageParticles.SendEvent("OnDamage");
    }

    private void Update()
    {
        float phase = _localTime / _duration;
        if (phase > 1)
        {
            enabled = false;
            _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
            _bodyMaterial.SetFloat("_Damage", 1f);
            _attackZoneMaterial.SetFloat("_Alpha", 0f);
            return;
        }
        _bodyMaterial.SetFloat("_AttackSemaphore", (1-phase)*0.05f);
        _bodyMaterial.SetFloat("_Damage", 1.25f);
        _attackZoneMaterial.SetFloat("_Alpha", 1-Mathf.Clamp(phase*3f, 0, 1));
        _localTime += Time.deltaTime;
    }
}