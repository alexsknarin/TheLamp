using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class LampDeathAnimation : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private AnimationCurve _damageAnimCurve;
    [SerializeField] private LampHealthBar _lampHealthBar;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private GameObject _lampAttackZoneObject;
    
    [Header("Fractured Glass")]
    [SerializeField] private GameObject _fracturedGlassBaseObject;
    [SerializeField] private GameObject _fracturedGlassObject;
    [SerializeField] private float _destructionSpeed;
    [SerializeField] private float _gravityStartAcceleration = 0.01f;
    [SerializeField] private float _rotationSpeed = 1f;
    private float _gravityAcceleration;
    [SerializeField] private Transform[] _bones;
    
    private Vector3[] _boneOriginalPositions;
    private Quaternion[] _boneOriginalRotations;
    
    private Vector3[] _boneDirections;
    private float[] _boneRotations;
    private float[] _boneSpeeds;
    private Vector3 _hitDirection;
    private Vector3 _fallDirection;
    

    private bool _isPlaying = false;
    private float _duration;
    private float _localTime = 0;

    private void Awake()
    {
        _boneDirections = new Vector3[_bones.Length];
        _boneRotations = new float[_bones.Length];
        _boneSpeeds = new float[_bones.Length];
        _boneOriginalPositions = new Vector3[_bones.Length];
        _boneOriginalRotations = new Quaternion[_bones.Length];
        for (int i = 0; i < _bones.Length; i++)
        {
            _boneOriginalPositions[i] = _bones[i].position;
            _boneOriginalRotations[i] = _bones[i].localRotation;
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i].position = _boneOriginalPositions[i];
            _bones[i].localRotation = _boneOriginalRotations[i];
        }
        for (int i = 0; i < _bones.Length; i++)
        {
            _boneDirections[i] = _bones[i].position.normalized;
            _boneRotations[i] = Random.Range(-270, 270) * _rotationSpeed;
            _boneSpeeds[i] = Random.Range(0.9f, 1.9f);
        }
        _lampEmissionController.ShowGlass();
        _fracturedGlassBaseObject.SetActive(false);
        _fracturedGlassObject.SetActive(false);

        _gravityAcceleration = _gravityStartAcceleration;
    }
    
    public void Play(float duration, Vector3 enemyPosition)
    {
        StartCoroutine(DisableAttackZone());
        
        _lampEmissionController.Intensity = 1;
        _lampEmissionController.IsDamageEnabled = true;
        _lampEmissionController.DamageMix = 1;
        
        // Glass destruction Setup
        _lampEmissionController.HideGlass();
        _fracturedGlassObject.SetActive(true);
        _fracturedGlassBaseObject.SetActive(true);
        
        _fracturedGlassObject.transform.position = transform.position;
        _fracturedGlassObject.transform.localRotation = transform.localRotation;
        
                
        _hitDirection = enemyPosition.normalized;
        _hitDirection.x *= -1;
        _hitDirection.y = -Mathf.Abs(_hitDirection.y);
        if (Mathf.Abs(_hitDirection.x) > 0.55f)
        {
            _hitDirection.x = Mathf.Sign(_hitDirection.x) * 0.525321f;
            _hitDirection.y = Mathf.Sign(_hitDirection.y) * 0.85f;
            _hitDirection = _hitDirection.normalized;
        }
        
        
        _duration = duration;
        _localTime = 0;
        _isPlaying = true;
    }
    
    private IEnumerator DisableAttackZone()
    {
        yield return null;
        _lampAttackZoneObject.SetActive(false);
    }
    
    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isPlaying = false;
                _lampEmissionController.Intensity = 0;
                _lampEmissionController.DamageMix = 0;
                _lampEmissionController.IsDamageEnabled = false;
                
                _lampHealthBar.UpdateHealth(0, 0);
                return;
            }
        
            float phaseAnimated = _animCurve.Evaluate(phase);
            _lampEmissionController.Intensity = phaseAnimated;
            _lampEmissionController.DamageMix = _damageAnimCurve.Evaluate(phase);
            
            _fallDirection = Vector3.Lerp(_hitDirection, Vector3.down, phase).normalized;
            
            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].position += _boneDirections[i] * (_destructionSpeed * _boneSpeeds[i] * Time.deltaTime) + _fallDirection * _gravityAcceleration;
                _gravityAcceleration += 0.0019f * Time.deltaTime;
                _bones[i].Rotate(Vector3.forward, _boneRotations[i] * Time.deltaTime);
            }
        
            _localTime += Time.deltaTime;
        }
    }
}
