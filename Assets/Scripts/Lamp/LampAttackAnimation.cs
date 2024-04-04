using UnityEngine;

public class LampAttackAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _lampLight;
    [SerializeField] private GameObject _lampAttackZoneObject;
    private Material _lampAttackZoneMaterial;
    private Material _lampMaterial;
    private bool _isPlaying = false;
    private float _localTime;
    private float _duration;
    private bool _isBlockedAttack = false;
    private float _lightMinimumIntensity;
    private float _lampMinimumEmission;
    private float _lightPower;
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
        _lightMinimumIntensity = lightMinimumIntensity;
        _lampMinimumEmission = lampMinimumEmission;
        _lightMaximumIntensity = lightMaximumIntensity;
        _lampMaximumEmission = lampMaximumEmission;
        
        _lampMaterial.SetFloat("_attackPower", 0f);
        _lightPower = Mathf.Pow(currentPower, 2f);
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
                _lampAttackZoneMaterial.SetFloat("_Alpha", 0);
                _isBlockedAttack = false;
                return;
            }
            if(!_isBlockedAttack)
            {
                _lampLight.intensity = Mathf.Lerp(_lightMaximumIntensity * _lightPower, _lightMinimumIntensity, phase);
                _lampMaterial.SetFloat("_EmissionLevel", Mathf.Lerp(_lampMaximumEmission * _lightPower, _lampMinimumEmission, phase));
                _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(_lightPower, 0, phase));
            }
            _localTime += Time.deltaTime;    
        }
    }
}
