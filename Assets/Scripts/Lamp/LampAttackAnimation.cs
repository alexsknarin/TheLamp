using System;
using UnityEngine;

public class LampAttackAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject _lampAttackZoneObject;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _emissionPowerCurve;
    [SerializeField] private AnimationCurve _attackZonePowerCurve;
    
    private Material _lampAttackZoneMaterial;
    private bool _isPlaying = false;
    private float _localTime;
    private float _duration;
    private bool _isBlockedAttack = false;
    private float _lightPower;
    private float _attackZonePower;

    public void Initialize()
    {
        _lampAttackZoneMaterial = _lampAttackZoneObject.GetComponent<MeshRenderer>().material;
    }
    
    public void Play(float duration, bool isBlockedAttack, float currentPower, float attackDistance)
    {
        _isBlockedAttack = isBlockedAttack;
        _lightPower = _emissionPowerCurve.Evaluate(currentPower);
        _attackZonePower = _attackZonePowerCurve.Evaluate(currentPower);
        _lampAttackZoneObject.transform.localScale = Vector3.one * (attackDistance * 2);
        _isPlaying = true;
        _duration = duration;
        _localTime = 0;
    }

    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isPlaying = false;
                _lampEmissionController.Intensity = 0f;
                _lampAttackZoneMaterial.SetFloat("_Alpha", 0);
                _isBlockedAttack = false;
                return;
            }

            if (!_isBlockedAttack)
            {
                _lampEmissionController.Intensity = Mathf.Lerp(_lightPower, 0, phase);
                _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(_attackZonePower, 0, phase));
            }

            _localTime += Time.deltaTime;    
        }
    }
}
