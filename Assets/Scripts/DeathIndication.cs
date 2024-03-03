using UnityEngine;

public class DeathIndication : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] float _duration = 0.5f;
    private Material _material;
    private bool _isFading = false;
    private float _prevTime;
    
    private void Start()
    {
        _material = _meshRenderer.material;
        
    }

    public void Perform()
    {
        _isFading = true;
        _prevTime = Time.time;
    }

    private void Update()
    {
        if (_isFading)
        {
            float phase = (Time.time - _prevTime) / _duration;
            _material.SetFloat("_AttackSemaphore", 1-phase);
            if (phase >= 1)
            {
                _isFading = false;
                _material.SetFloat("_AttackSemaphore", 0);
            }            
        }
    }
}
