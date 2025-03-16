using UnityEngine;

public class AttackDistanceUpgradeAnimationController : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _material;
    private float _duration;
    private float _localTime;
    private bool _isPlaying = false;
    

    public void Initialize()
    {
        _material = _meshRenderer.material;
    }

    public void Play(float duration)
    {
        _duration = duration;
        _localTime = 0;
        _isPlaying = true;
    }

    private void Update()
    {
        if (_isPlaying)
        {
            if (_localTime > _duration)
            {
                _isPlaying = false;
                _material.SetFloat("_Upgrade", 0);
                enabled = false;
            }
            _material.SetFloat("_Upgrade", 1 - _localTime/_duration);    
            _localTime += Time.deltaTime;
        }
    }
}
