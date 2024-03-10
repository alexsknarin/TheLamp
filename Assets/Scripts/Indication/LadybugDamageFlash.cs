using UnityEngine;

public class LadybugDamageFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshRenderer _attackZone;
    [SerializeField] private float _duration = 0.5f;
    
    private Material _bodyMaterial;
    private Material _attackZoneMaterial;
    private bool _isActive = false;
    private float _prevTime;
    
    public override void Play()
    {
        _isActive = true;
        _prevTime = Time.time;
    }

    void Update()
    {
        if (_isActive)
        {
            float phase = (Time.time - _prevTime) / _duration;
            if (phase > 1)
            {
                _isActive = false;
                _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
                _bodyMaterial.SetFloat("_Damage", 1f);
                _attackZoneMaterial.SetFloat("_Alpha", 0f);
                return;
            }
            _bodyMaterial.SetFloat("_AttackSemaphore", (1-phase)*0.05f);
            _bodyMaterial.SetFloat("_Damage", 1.25f);
            _attackZoneMaterial.SetFloat("_Alpha", 1-Mathf.Clamp(phase*3f, 0, 1));
        }
    }
    
    public override void Initialize()
    {
        _bodyMaterial = _meshRenderer.material;
        _attackZoneMaterial = _attackZone.material;
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _bodyMaterial.SetFloat("_Damage", 1f);
        _attackZoneMaterial.SetFloat("_Alpha", 0f);
    }
}