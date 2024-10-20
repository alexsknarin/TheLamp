using UnityEngine;

public class DragonflyPreAttackFlash : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _bodyMeshRenderer;
    [SerializeField] private MeshRenderer _wingsMeshRenderer;
    // [SerializeField] private bool _enableTrailRenderer;
    // [SerializeField] private TrailRenderer _trailRenderer;
    private Material _bodyMaterial;
    private Material _wingsMaterial;
    // private Material _trailMaterial;
    
    public void PreAttackStart()
    {
        _bodyMaterial.SetFloat("_AttackSemaphore", 1f);
        _wingsMaterial.SetFloat("_AttackSemaphore", 1f);
        // if (_enableTrailRenderer)
        // {
        //     _trailMaterial.SetFloat("_EmissionMultipler", .1f);    
        // }
    }
    
    public void PreAttackEnd()
    {
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
        // if (_enableTrailRenderer)
        // {
        //     _trailMaterial.SetFloat("_EmissionMultipler", 0f);    
        // }
        
    }

    public void Initialize()
    {
        _bodyMaterial = _bodyMeshRenderer.material;
        _wingsMaterial = _wingsMeshRenderer.material;
        
        // if (_enableTrailRenderer)
        // {
        //     _trailMaterial = _trailRenderer.material;
        //     _trailMaterial.SetFloat("_EmissionMultipler", 0f);
        // }
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
    }
}
