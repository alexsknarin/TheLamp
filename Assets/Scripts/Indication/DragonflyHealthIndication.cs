using UnityEngine;

public class DragonflyHealthIndication : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _bodyMeshRenderer;
    [SerializeField] private MeshRenderer _wingsMeshRenderer;
    private Material _bodyMaterial;
    private Material _wingsMaterial;
    
    public void Refresh(int currentHealth, int maxHealth)
    {
        float damagePhase = ((float)(maxHealth - currentHealth) / maxHealth) * 0.5f;
        _bodyMaterial.SetFloat("_DamagePhase", damagePhase);
        _wingsMaterial.SetFloat("_DamagePhase", damagePhase);
    }
    
    public void Initialize()
    {
        _bodyMaterial = _bodyMeshRenderer.material;
        _wingsMaterial = _wingsMeshRenderer.material;
        _bodyMaterial.SetFloat("_DamagePhase", 0f);
        _wingsMaterial.SetFloat("_DamagePhase", 0f);
    }
}
