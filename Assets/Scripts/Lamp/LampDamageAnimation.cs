using UnityEngine;

public class LampDamageAnimation : MonoBehaviour, IInitializable
{
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
                _lampMaterial.SetFloat("_Damage", 0f);
                return;
            }
            _lampMaterial.SetFloat("_Damage", Mathf.Lerp(1f, 0f, phase));
            _localTime += Time.deltaTime;    
        }
    }
}
