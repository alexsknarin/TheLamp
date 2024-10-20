using UnityEngine;
// using UnityEngine.VFX;

public class DragonflyDeathFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _bodyMeshRenderer;
    [SerializeField] private MeshRenderer _wingsMeshRenderer;
    [SerializeField] private float _duration = 1.7f;
    // [SerializeField] private VisualEffect _deathParticles;
    // [SerializeField] private VisualEffect _damageParticles;
    private Material _bodyMaterial;
    private Material _wingsMaterial;
    private bool _isActive = false;
    private float _localTime;
    
    public override void Play()
    {
        _isActive = true;
        _localTime = 0;
        _bodyMaterial.SetFloat("_DeathPhase", 0f);
        _wingsMaterial.SetFloat("_DeathPhase", 0f);
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
        
        // Vector3 direction = transform.position.normalized;
        // _deathParticles.gameObject.SetActive(true);
        // _damageParticles.SetVector3("Direction", direction);
        // _deathParticles.SendEvent("OnDeathStart");
        // _damageParticles.SendEvent("OnDamage");
    }

    void Update()
    {
        if (_isActive)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isActive = false;
                _bodyMaterial.SetFloat("_DeathPhase", 1f);
                _wingsMaterial.SetFloat("_DeathPhase", 1f);
                
                // _deathParticles.SendEvent("OnDeathStart");
                // _deathParticles.gameObject.SetActive(false);
                return;
            }
            _bodyMaterial.SetFloat("_DeathPhase", phase);
            _wingsMaterial.SetFloat("_DeathPhase", phase);
            
            _localTime += Time.deltaTime;
        }
    }

    public override void Initialize()
    {
        _isActive = false;
        
        _bodyMaterial = _bodyMeshRenderer.material;
        _wingsMaterial = _wingsMeshRenderer.material;
        
        _bodyMaterial.SetFloat("_DeathPhase", 0f);
        _wingsMaterial.SetFloat("_DeathPhase", 0f);
        
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
        
        // _deathParticles.gameObject.SetActive(false);
    }
}