using System;
using UnityEngine;

public class LampAttackDistanceUpgradeAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private MeshRenderer _attackZonePresentationRenderer;
    [SerializeField] private float _duration;
    private Material _attackZonePresentationMaterial;
    private bool _isPlaying = false;
    private float _localTime = 0;
    private float _originalAlpha;
    private float _newAlpha;
    
    
    public void Initialize()
    {
        _attackZonePresentationMaterial = _attackZonePresentationRenderer.material;
        _isPlaying = false;
    }
    
    public void Play()
    {
        _localTime = 0;
        _isPlaying = true;
        _originalAlpha = _attackZonePresentationMaterial.GetFloat("_Alpha");
        _newAlpha = _originalAlpha * 2.4f;
    }

    private void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if(phase > 1)
            {
                _isPlaying = false;
                _attackZonePresentationMaterial.SetFloat("_Upgrade", 1);
                _attackZonePresentationMaterial.SetFloat("_Alpha", _originalAlpha);
                return;
            }
            _attackZonePresentationMaterial.SetFloat("_Upgrade", phase);
            _attackZonePresentationMaterial.SetFloat("_Alpha", Mathf.Lerp(_newAlpha, _originalAlpha, phase));
            _localTime += Time.deltaTime;
        }
    }
}
