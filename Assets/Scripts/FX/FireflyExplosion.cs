using UnityEngine;

public class FireflyExplosion : MonoBehaviour
{
    // TODO: redesign as Gameplay VIEW
    [SerializeField] private AnimationCurve _explosionCurve;
    [SerializeField] private AnimationCurve _explosionMaterialCurve;
    [SerializeField] private MeshRenderer _meshRenderer;
    private Material _material;
    private bool _isActive;
    private float _phase;
    private float _baseScale = 1;
    private float _localTime;
    private float _duration;

    public void Play(Vector3 position, float radius, float duration)
    {
        transform.position = position;
        gameObject.SetActive(true);
        _isActive = true;
        _material.SetFloat("_ExplosionPhase", 0);
        _baseScale = radius;
        _duration = duration;
        _localTime = 0;
    }

    private void Awake()
    {
        _material = _meshRenderer.material;
    }

    private void Update()
    {
        if (_isActive)
        {
            _phase = _localTime / _duration;
            if (_phase >= 1)
            {
                _isActive = false;
                _material.SetFloat("_ExplosionPhase", 0);
                gameObject.SetActive(false);
            }
            else
            {
                float curveValue = _explosionCurve.Evaluate(_phase);
                Vector3 scale = Vector3.one * (curveValue * _baseScale);
                transform.localScale = scale;
                _material.SetFloat("_ExplosionPhase", _explosionMaterialCurve.Evaluate(_phase));
            }
            _localTime += Time.deltaTime;
        }
    }
}
