using UnityEngine;

public class HealthIndication : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _material;

    public void Refresh(int currentHealth, int maxHealth)
    {
        _material.SetFloat("_Health", (float)currentHealth / maxHealth);
    }

    public void Initialize()
    {
        _material = _meshRenderer.material;
        _material.SetFloat("_Health", 1f);
    }
}
