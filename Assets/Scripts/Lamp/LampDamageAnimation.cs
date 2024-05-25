using UnityEngine;

public class LampDamageAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private LampEmissionController _lampEmissionController;
    private Material _lampMaterial;
    private bool _isPlaying = false;
    private float _duration;
    private float _localTime = 0;

    public void Initialize()
    {
        _lampMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void Play(float duration)
    {
        _lampMaterial.SetFloat("_Damage", 1f);
        
        _lampEmissionController.IsDamageEnabled = true;
        _lampEmissionController.DamageMix = 1f;
        
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
                _lampMaterial.SetFloat("_Damage", 0f);
                
                
                _lampEmissionController.DamageMix = 0f;
                _lampEmissionController.IsDamageEnabled = false;
                _lampEmissionController.Intensity = 0f;
                _isPlaying = false;
                return;
            }
            _lampMaterial.SetFloat("_Damage", Mathf.Lerp(1f, 0f, phase));
            
            // NEW LAMP
            _lampEmissionController.DamageMix = 1f - Mathf.Clamp(phase * 1.5f, 0, 1);
            _lampEmissionController.Intensity = 1f - phase;
            
            _localTime += Time.deltaTime;    
        }
    }
}
