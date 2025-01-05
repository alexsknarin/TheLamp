using UnityEngine;

public class LampEmissionController : MonoBehaviour, IInitializable
{
    [field: SerializeField, Range(0f, 10f)] public float Intensity { set; get; }
    [field: SerializeField] public bool IsBlockedMode { set; get; }
    [field: SerializeField, Range(0f, 1f)] public float BlockedModeMix { set; get; }
    [SerializeField] private float _blockedModeNoseFrequency;
    [field: Header("--------")]
    [field: SerializeField] public bool IsDamageEnabled { set; get; }
    [field: SerializeField, Range(0f, 1f)] public float DamageMix { set; get; }
    [Header("--------")]
    [SerializeField] private MeshRenderer _lampInternalMeshRenderer;
    [SerializeField] private MeshRenderer _lampGlassMeshRenderer;
    [SerializeField] private MeshRenderer _lampSocketMeshRenderer;
    [SerializeField] private Light _lampLight;
    private Material _filamentMaterial;
    private Material _electrodeMaterial;
    private Material _glassTubeMaterial;
    private Material _lampGlassMaterial;
    private Material _lampSocketAluminiumMaterial;
    private readonly float _lightNeutralIntensity = 22;
    private readonly float _lightMinimumIntensity = 0.1f;
    private Color _ligtMinimumColor = new Color(0.8301f, 0.268f, 0.1331f);
    private Color _ligtMaximumColor = new Color(0.9058824f, 0.6f, 0.3764f);
    private Color _ligtDamageColor = new Color(0.931f, 0.1254f, 0.0671f);

    public void Initialize()
    {
        Material[] materials = _lampInternalMeshRenderer.materials;
        _filamentMaterial = materials[0];
        _electrodeMaterial = materials[1];
        _glassTubeMaterial = materials[2];
        _lampGlassMaterial = _lampGlassMeshRenderer.sharedMaterial;
        _lampSocketAluminiumMaterial = _lampSocketMeshRenderer.materials[0];
        
        _filamentMaterial.SetFloat("_DamageMix", 0);
        _electrodeMaterial.SetFloat("_DamageMix", 0);
        _glassTubeMaterial.SetFloat("_DamageMix", 0);
        _lampGlassMaterial.SetFloat("_DamageMix", 0);
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
    
    public void LampGlassDamageUpdate(GlassDamageData glassDamageData)
    {
        _lampGlassMaterial.SetFloat("_CracksAmountR", glassDamageData.CracksAmountRight);
        _lampGlassMaterial.SetFloat("_CracksAmountL", glassDamageData.CracksAmountLeft);
        _lampGlassMaterial.SetFloat("_CracksAmountB", glassDamageData.CracksAmountBottom);
        
        _lampGlassMaterial.SetFloat("_ImpactPoint01Strength", glassDamageData.LampDamagePoint01.Strength);
        _lampGlassMaterial.SetFloat("_ImpactPoint01LocalAngle", glassDamageData.LampDamagePoint01.LocalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint01GlobalAngle", glassDamageData.LampDamagePoint01.GlobalAngle);
        
        _lampGlassMaterial.SetFloat("_ImpactPoint02Strength", glassDamageData.LampDamagePoint02.Strength);
        _lampGlassMaterial.SetFloat("_ImpactPoint02LocalAngle", glassDamageData.LampDamagePoint02.LocalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint02GlobalAngle", glassDamageData.LampDamagePoint02.GlobalAngle);
        
        _lampGlassMaterial.SetFloat("_ImpactPoint03Strength", glassDamageData.LampDamagePoint03.Strength);
        _lampGlassMaterial.SetFloat("_ImpactPoint03LocalAngle", glassDamageData.LampDamagePoint03.LocalAngle);
        _lampGlassMaterial.SetFloat("_ImpactPoint03GlobalAngle", glassDamageData.LampDamagePoint03.GlobalAngle);
        
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

    public static float LerpExtrapolated( float a, float b, float t ){
        return t*b + (1-t)*a;
    }

    private void Update()
    {
        float intensity = Intensity;

        if (IsBlockedMode)
        {
            intensity = BlockedModeNoise(Intensity, 1f);
        }
        else if (BlockedModeMix > 0)
        {
            intensity = BlockedModeNoise(intensity, BlockedModeMix);
        }
        
        _filamentMaterial.SetFloat("_EmissionStrength", intensity * 0.4f);
        _electrodeMaterial.SetFloat("_EmissionStrength", intensity*1.6f);
        _glassTubeMaterial.SetFloat("_EdgeEmissionStrength", intensity*1.6f);
        _lampGlassMaterial.SetFloat("_EmissionStrength", intensity*1.6f);
        _lampSocketAluminiumMaterial.SetFloat("_EmissionStrength", intensity);
        
        _lampLight.intensity = LerpExtrapolated(_lightMinimumIntensity, _lightNeutralIntensity, intensity);
        _lampLight.color = Color.Lerp(_ligtMinimumColor, _ligtMaximumColor, intensity);

        if (IsDamageEnabled)
        {
            _filamentMaterial.SetFloat("_DamageMix", DamageMix);
            _electrodeMaterial.SetFloat("_DamageMix", DamageMix);
            _glassTubeMaterial.SetFloat("_DamageMix", DamageMix);
            _lampGlassMaterial.SetFloat("_DamageMix", DamageMix);
            _lampSocketAluminiumMaterial.SetFloat("_DamageMix", DamageMix);
            _lampLight.color = Color.Lerp(_lampLight.color, _ligtDamageColor, DamageMix);
        }
    }

    private float BlockedModeNoise(float intensity, float mix)
    {
        float blockedNoise = 1;
        blockedNoise = Mathf.PerlinNoise1D(Time.time * _blockedModeNoseFrequency) * 1.35f - 0.35f;
        blockedNoise = Mathf.Clamp01(blockedNoise);
        blockedNoise = Mathf.Lerp(1, blockedNoise, mix);
        return intensity * blockedNoise;
    }
}
