using UnityEngine;
using UnityEngine.VFX;

public class DragonflyHealthIndication : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _bodyMeshRenderer;
    [SerializeField] private MeshRenderer _wingsMeshRenderer;
    [SerializeField] private VisualEffect _damageEmitParticles;
    [SerializeField] private float _damageEmitRate = 22f;
    [SerializeField] private float _damageLifeMin = 0.1f;
    [SerializeField] private float _damageLifeMax = 0.45f;
    private Material _bodyMaterial;
    private Material _wingsMaterial;
    private bool _isParticleSystemActive = false;
    
    public void Refresh(int currentHealth, int maxHealth)
    {
        float damagePhase = ((float)(maxHealth - currentHealth) / maxHealth) * 0.5f;
        _bodyMaterial.SetFloat("_DamagePhase", damagePhase);
        _wingsMaterial.SetFloat("_DamagePhase", damagePhase);

        if (damagePhase > 0.05f && !_isParticleSystemActive)
        {
            _isParticleSystemActive = true;
            _damageEmitParticles.gameObject.SetActive(true);
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", damagePhase * _damageEmitRate);
        }

        if (_isParticleSystemActive)
        {
            _damageEmitParticles.SetFloat("Rate", damagePhase * _damageEmitRate);
        }
    }
    
    public void Initialize()
    {
        _bodyMaterial = _bodyMeshRenderer.material;
        _wingsMaterial = _wingsMeshRenderer.material;
        _bodyMaterial.SetFloat("_DamagePhase", 0f);
        _wingsMaterial.SetFloat("_DamagePhase", 0f);
        
        _isParticleSystemActive = false;
        _damageEmitParticles.SendEvent("OnEndEmit");
        _damageEmitParticles.SetFloat("Rate", 0);
        _damageEmitParticles.SetFloat("LifeMin", _damageLifeMin);
        _damageEmitParticles.SetFloat("LifeMax", _damageLifeMax);
        _damageEmitParticles.gameObject.SetActive(false);
    }
}
