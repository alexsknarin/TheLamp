using UnityEngine;

public class LampHealthBarController : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _healthBarMeshRenderer;
    private Transform _healthBarTransform;
    private Material _healthBarMaterial;
    private float _localTime;
    private bool _isHealthBarUpgradePlaying = false;
    private float _healthBarUpgradeDuration = 0.8f;

    public void Initialize()
    {
        _healthBarTransform = transform;
        _healthBarMaterial = _healthBarMeshRenderer.material;
    }
    
    public void SetHealth(float health)
    {
        _healthBarMaterial.SetFloat("_Health", health);
        Vector3 rotation = Vector3.zero;
        rotation.y = Mathf.Lerp(109, 0, health);
        _healthBarTransform.localEulerAngles = rotation;
    }
    
    public void EnableLastHealth()
    {
        _healthBarMaterial.SetInt("_isLastHealth", 1);
    }
    
    public void DisableLastHealth()
    {
        _healthBarMaterial.SetInt("_isLastHealth", 0);
    }
    
    public void PlayUpgrade()
    {
        _isHealthBarUpgradePlaying = true;
        _localTime = 0;
    }
    
    private void PerformUpgrade()
    {
        float phase = _localTime / _healthBarUpgradeDuration;
        if (phase > 1)
        {
            _isHealthBarUpgradePlaying = false;
            _healthBarMaterial.SetFloat("_HealthUpgrade", 0);
            return;
        }
        _healthBarMaterial.SetFloat("_HealthUpgrade", Mathf.Sin(phase * Mathf.PI));
        _localTime += Time.deltaTime;
    }

    private void Update()
    {
        if (_isHealthBarUpgradePlaying)
        {
            PerformUpgrade();
        }
    }
}
