using UnityEngine;

public class DamageFlash : DamageIndication
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _duration = 0.5f;
    private Material _material;
    private bool _isActive = false;
    private float _prevTime;
    
    public override void StartPerform()
    {
        _isActive = true;
        _prevTime = Time.time;
    }

    void Update()
    {
        if (_isActive)
        {
            float phase = (Time.time - _prevTime) / _duration;
            if (phase > 1)
            {
                _isActive = false;
                _material.SetFloat("_AttackSemaphore", 0f);
                _material.SetFloat("_Damage", 1f);
                return;
            }
            _material.SetFloat("_AttackSemaphore", 1-phase);
            _material.SetFloat("_Damage", 9f);
        }
    }
    
    public override void Initialize()
    {
        _material = _meshRenderer.material;
        _material.SetFloat("_AttackSemaphore", 0f);
        _material.SetFloat("_Damage", 1f);
    }
}
