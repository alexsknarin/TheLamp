using UnityEngine;

public class PreAttackFlash : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private TrailRenderer _trailRenderer;
    private Material _bodyMaterial;
    private Material _trailMaterial;
    
    public void PreAttackStart()
    {
        _bodyMaterial.SetFloat("_AttackSemaphore", 1f);
        _trailMaterial.SetFloat("_EmissionMultipler", 1f);
    }
    
    public void PreAttackEnd()
    {
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);   
        _trailMaterial.SetFloat("_EmissionMultipler", 0f);
    }

    public void Initialize()
    {
        _bodyMaterial = _meshRenderer.material;
        _trailMaterial = _trailRenderer.material;
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _trailMaterial.SetFloat("_EmissionMultipler", 0f);
    }
}
