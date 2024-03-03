using UnityEngine;

public class PreAttackFlash : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _material;
    
    // private void Start()
    // {
    //     _material = _meshRenderer.material;
    // }

    public void PreAttackStart()
    {
        _material.SetFloat("_AttackSemaphore", 1f);
    }
    
    public void PreAttackEnd()
    {
        _material.SetFloat("_AttackSemaphore", 0f);   
    }

    public void Initialize()
    {
        _material = _meshRenderer.material;
        _material.SetFloat("_AttackSemaphore", 0f);
    }
}
