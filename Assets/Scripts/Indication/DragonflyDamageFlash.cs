using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DragonflyDamageFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _bodyMeshRenderer;
    [SerializeField] private MeshRenderer _wingsMeshRenderer;
    [SerializeField] private float _duration = 1.2f;
    [SerializeField] private VisualEffect _damageParticles;
    private Material _bodyMaterial;
    private Material _wingsMaterial;
    private WaitForSeconds _damageFlashDuration = new WaitForSeconds(1.2f); // TODO: make use parameter - initialize in Initialize()
    private Transform _contactCollisionTransform;

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
        
        if (_contactCollisionTransform != null)
        {
            _damageParticles.transform.localPosition = _contactCollisionTransform.localPosition;
            Vector3 direction = -_contactCollisionTransform.position.normalized;
            _damageParticles.SetVector3("Direction", direction);
            _damageParticles.SendEvent("OnDamage");
        }
    }

    public void SetContactCollisionTransform(Transform contactCollisionTransform)
    {
        _contactCollisionTransform = contactCollisionTransform;
    }

    private IEnumerator WaitForDamageFlashEnd()
    {
        yield return _damageFlashDuration;
        _bodyMaterial.SetInt("_isDamaged", 0);
        _wingsMaterial.SetInt("_isDamaged", 0);
    }
}
