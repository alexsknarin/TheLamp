using UnityEngine;

public class LampAttackAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _lampLight;
    // NEW LAMP
    [SerializeField] private GameObject _lampAttackZoneObject;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _emissionPowerCurve;
    [SerializeField] private AnimationCurve _attackZonePowerCurve;
    
    private Material _lampAttackZoneMaterial;
    private Material _lampMaterial;
    private bool _isPlaying = false;
    private float _localTime;
    private float _duration;
    private bool _isBlockedAttack = false;
    private float _lightMinimumIntensity;
    private float _lampMinimumEmission;
    private float _lightPower;
    private float _attackZonePower;
    private float _lightMaximumIntensity;
    private float _lampMaximumEmission;

    public void Initialize()
    {
        _lampMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        _lampAttackZoneMaterial = _lampAttackZoneObject.GetComponent<MeshRenderer>().material;
    }
    
    public void Play(float duration, bool isBlockedAttack, float lightMinimumIntensity, float lampMinimumEmission, 
        float currentPower, float lightMaximumIntensity, float lampMaximumEmission,
        float attackDistance)
    {
        _isBlockedAttack = isBlockedAttack;
        if (_isBlockedAttack)
        {
            _lampEmissionController.BlockedModeMix = 0.9f;
        }
        
        Debug.Log(_isBlockedAttack);
        
        _lightMinimumIntensity = lightMinimumIntensity;
        _lampMinimumEmission = lampMinimumEmission;
        _lightMaximumIntensity = lightMaximumIntensity;
        _lampMaximumEmission = lampMaximumEmission;
        
        _lampMaterial.SetFloat("_attackPower", 0f);
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
                _lampLight.intensity = _lightMinimumIntensity;
                _lampMaterial.SetFloat("_EmissionLevel", _lampMinimumEmission);
                
                // NEW LAMP
                
                _lampEmissionController.Intensity = 0f;
                _lampAttackZoneMaterial.SetFloat("_Alpha", 0);
                _isBlockedAttack = false;
                return;
            }

            if (!_isBlockedAttack)
            {
                _lampLight.intensity = Mathf.Lerp(_lightMaximumIntensity * _lightPower, _lightMinimumIntensity, phase);
                _lampMaterial.SetFloat("_EmissionLevel",
                    Mathf.Lerp(_lampMaximumEmission * _lightPower, _lampMinimumEmission, phase));

                // NEW LAMP
                _lampEmissionController.Intensity = Mathf.Lerp(_lightPower, 0, phase);
                _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(_attackZonePower, 0, phase));
            }

            _localTime += Time.deltaTime;    
        }
    }
}
