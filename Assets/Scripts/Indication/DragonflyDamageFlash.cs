using System.Collections;
using UnityEngine;

public class DragonflyDamageFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _bodyMeshRenderer;
    [SerializeField] private MeshRenderer _wingsMeshRenderer;
    [SerializeField] private float _duration = 0.5f;
    // [SerializeField] private VisualEffect _damageParticles;
    private Material _bodyMaterial;
    private Material _wingsMaterial;
    private WaitForSeconds _damageFlashDuration = new WaitForSeconds(1.2f);
    
    private IEnumerator WaitForDamageFlashEnd()
    {
        yield return _damageFlashDuration;
        _bodyMaterial.SetInt("_isDamaged", 0);
        _wingsMaterial.SetInt("_isDamaged", 0);
    }
    public override void Initialize()
    {
        _bodyMaterial = _bodyMeshRenderer.material;
        _wingsMaterial = _wingsMeshRenderer.material;
        _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
        _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
    }
    
    public override void Play()
    {
        
        _bodyMaterial.SetInt("_isDamaged", 1);
        _wingsMaterial.SetInt("_isDamaged", 1);
        StartCoroutine(WaitForDamageFlashEnd());
        
        
        // Vector3 direction = transform.position.normalized;
        // _damageParticles.SetVector3("Direction", direction);
        // _damageParticles.SendEvent("OnDamage");
    }
    
    
}
