using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampEmissionController : MonoBehaviour
{
    [Range(0f, 10f)]
    [SerializeField] public float _intensity = 0;
    public float Intensity
    {
        set => _intensity = value;
        get => _intensity;
    }
    
    [Range(0f, 1f)]
    [SerializeField] private float _blockedModeMix;
    public float BlockedModeMix
    {
        set => _blockedModeMix = value;
        get => _blockedModeMix;
    }
    [SerializeField] private float _blockedModeNoseFrequency;
    [Header("--------")]
    [SerializeField] private bool _isDamageEnabled;
    public bool IsDamageEnabled
    {
        set => _isDamageEnabled = value;
        get => _isDamageEnabled;
    }
    [Range(0f, 1f)]
    [SerializeField] private float _damageMix;
    public float DamageMix
    {
        set => _damageMix = value;
        get => _damageMix;
    }
    
    [Header("--------")]
    [SerializeField] private MeshRenderer _lampInternalMeshRenderer;
    private Material _filamentMaterial;
    private Material _electrodeMaterial;
    private Material _glassTubeMaterial;
    [SerializeField] private MeshRenderer _lampGlassMeshRenderer;
    private Material _lampGlassMaterial;
    [SerializeField] private MeshRenderer _lampSocketMeshRenderer;
    private Material _lampSocketAluminiumMaterial;
    [SerializeField] private Light _lampLight;

    private readonly float _lightNeutralIntensity = 22;
    private readonly float _lampNeutralEmission = 1f;
    
    private readonly float _lightMinimumIntensity = 0.1f;
    private readonly float _lampMinimumEmission = 0.01f;
    
    private readonly float _lightMaximumIntensity = 130;
    private readonly float _lampMaximumEmission = 35f;
    
    private Color _ligtMinimumColor = new Color(0.8301f, 0.268f, 0.1331f);
    private Color _ligtMaximumColor = new Color(0.9058824f, 0.6f, 0.3764f);
    private Color _ligtDamageColor = new Color(0.931f, 0.1254f, 0.0671f);
        
    private void Start()
    {
        Material[] materials = _lampInternalMeshRenderer.materials;
        _filamentMaterial = materials[0];
        _electrodeMaterial = materials[1];
        _glassTubeMaterial = materials[2];
        _lampGlassMaterial = _lampGlassMeshRenderer.sharedMaterial;
        _lampSocketAluminiumMaterial = _lampSocketMeshRenderer.materials[0];
    }

    public void HideGlass()
    {
        _lampGlassMeshRenderer.gameObject.SetActive(false);
    }

    public void ShowGlass()
    {
        _lampGlassMeshRenderer.gameObject.SetActive(true);
    }
    
    public void LampDamageUpdate(Vector3 damageWeights)
    {
        _lampGlassMaterial.SetFloat("_CracksAmountR", damageWeights.x);
        _lampGlassMaterial.SetFloat("_CracksAmountL", damageWeights.y);
        _lampGlassMaterial.SetFloat("_CracksAmountB", damageWeights.z);
    }
    
    public void LampImpactDamageUpdate(LampImpactPointsData impactPointsData)
    {
        _lampGlassMaterial.SetFloat("_ImpactPoint01Strength", impactPointsData.ImpactPoint01Strength);
        _lampGlassMaterial.SetFloat("_ImpactPoint01LocalAngle", impactPointsData.ImpactPoint01LocalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint01GlobalAngle", impactPointsData.ImpactPoint01GlobalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint02Strength", impactPointsData.ImpactPoint02Strength);
        _lampGlassMaterial.SetFloat("_ImpactPoint02LocalAngle", impactPointsData.ImpactPoint02LocalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint02GlobalAngle", impactPointsData.ImpactPoint02GlobalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint03Strength", impactPointsData.ImpactPoint03Strength);
        _lampGlassMaterial.SetFloat("_ImpactPoint03LocalAngle", impactPointsData.ImpactPoint03LocalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint03GlobalAngle", impactPointsData.ImpactPoint03GlobalAngle);
    }
    
    private void Update()
    {
        float blockedNoise = 1;
        float intensity = _intensity;
        if (_blockedModeMix > 0)
        {
            blockedNoise = Mathf.PerlinNoise1D(Time.time * _blockedModeNoseFrequency) * 1.35f - 0.35f;
            blockedNoise = Mathf.Clamp01(blockedNoise);
            blockedNoise = Mathf.Lerp(1, blockedNoise, _blockedModeMix);
            intensity = _intensity * blockedNoise;
        }
        
        _filamentMaterial.SetFloat("_EmissionStrength", intensity * 0.4f);
        _electrodeMaterial.SetFloat("_EmissionStrength", intensity*1.6f);
        _glassTubeMaterial.SetFloat("_EdgeEmissionStrength", intensity*1.6f);
        _lampGlassMaterial.SetFloat("_EmissionStrength", intensity*1.6f);
        _lampSocketAluminiumMaterial.SetFloat("_EmissionStrength", intensity);
        
        _lampLight.intensity = LerpExtrapolated(_lightMinimumIntensity, _lightNeutralIntensity, intensity);
        _lampLight.color = Color.Lerp(_ligtMinimumColor, _ligtMaximumColor, intensity);

        if (_isDamageEnabled)
        {
            _filamentMaterial.SetFloat("_DamageMix", _damageMix);
            _electrodeMaterial.SetFloat("_DamageMix", _damageMix);
            _glassTubeMaterial.SetFloat("_DamageMix", _damageMix);
            _lampGlassMaterial.SetFloat("_DamageMix", _damageMix);
            _lampSocketAluminiumMaterial.SetFloat("_DamageMix", _damageMix);
            _lampLight.color = Color.Lerp(_lampLight.color, _ligtDamageColor, _damageMix);
        }
    }
    
    public static float LerpExtrapolated( float a, float b, float t ){
        return t*b + (1-t)*a;
    }
}
