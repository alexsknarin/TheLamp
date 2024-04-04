using System;
using UnityEngine;

public class LampHealthBar : MonoBehaviour, IInitializable
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

    public void UpdateHealth(float normalizedHealth, int actualHealth)
    {
        _healthBarMaterial.SetFloat("_Health", normalizedHealth);
        if (actualHealth == 1)
        {
            _healthBarMaterial.SetInt("_isLastHealth", 1);    
        }
        else
        {
            _healthBarMaterial.SetInt("_isLastHealth", 0);
        }
        
        Vector3 scale = Vector3.one;
        scale.x = normalizedHealth;
        _healthBarTransform.localScale = scale;
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
