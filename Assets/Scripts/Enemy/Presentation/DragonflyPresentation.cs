using System.Collections.Generic;
using UnityEngine;

public class DragonflyPresentation : EnemyPresentation
{
    [SerializeField] private List<MeshRenderer> _meshRenderers;
    [ColorUsage(false, true)] [SerializeField] private List<Color> _baseColors;
    
    
    private bool _isDamageFlashing = false;
    private float _localTime = 0f;
    
    
    
    public override void PreAttackStart()
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
        {
            Color currentColor = Color.Lerp(Color.red, _baseColors[i], .5f);
            _meshRenderers[i].material.SetColor("_EmissionColor", currentColor);
        }
    }

    public override void PreAttackEnd()
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
        {
            _meshRenderers[i].material.SetColor("_EmissionColor", _baseColors[i]);
        }
    }

    public override void DamageFlash()
    {
        _isDamageFlashing = true;
        _localTime = 0f;
    }

    public override void DeathFlash()
    {
        throw new System.NotImplementedException();
    }

    public override void HealthUpdate(int currentHealth, int maxHealth)
    {
        throw new System.NotImplementedException();
    }

    public override void Initialize()
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
        {
            _meshRenderers[i].material.SetColor("_EmissionColor", _baseColors[i]);
        }
    }

    private void Update()
    {
        if (_isDamageFlashing)
        {
            float phase = _localTime / 0.5f;
            
            if (phase > 1)
            {
                _isDamageFlashing = false;
                for (int i = 0; i < _meshRenderers.Count; i++)
                {
                    _meshRenderers[i].material.SetColor("_EmissionColor", _baseColors[i]);
                }
                return;
            }
            
            for (int i = 0; i < _meshRenderers.Count; i++)
            {
                Color currentColor = Color.Lerp(Color.red, _baseColors[i], phase);
                _meshRenderers[i].material.SetColor("_EmissionColor", currentColor);
            }
            
            _localTime += Time.deltaTime;
        }
    }
}
