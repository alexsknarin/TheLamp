using UnityEngine;

public class DragonflyProjectilePresentation : EnemyPresentation
{
    // TODO: replace with a standard Death Flash
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _duration = 1.7f;
    private Material _material;
    private bool _isActive = false;
    private float _localTime;
    
    
    public override void Initialize()
    {
        _isActive = false;
        _material = _meshRenderer.material;
        _material.SetFloat("_DeathFade", 0f);
        _material.SetFloat("_AttackSemaphore", 0f);
        _material.SetFloat("_Damage", 1f);
    }
    
    public override void PreAttackStart()
    {
        throw new System.NotImplementedException();
    }

    public override void PreAttackEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void DamageFlash()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        if (_isActive)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isActive = false;
                _material.SetFloat("_DeathFade", 1f);
                _material.SetFloat("_Damage", 1f);
                return;
            }
            _material.SetFloat("_Damage", 4f);
            _material.SetFloat("_DeathFade", phase);
            _localTime += Time.deltaTime;
        }
    }

    public override void DeathFlash()
    {
        _isActive = true;
        _localTime = 0;
        _material.SetFloat("_DeathFade", 0);
        _material.SetFloat("_AttackSemaphore", 0);
        _material.SetFloat("_Damage", 1f);
    }

    public override void HealthUpdate(int currentHealth, int maxHealth)
    {
        throw new System.NotImplementedException();
    }


}
