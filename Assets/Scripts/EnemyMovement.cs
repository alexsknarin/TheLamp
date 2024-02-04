using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

enum NoiseType
{
    WorldSpace,
    AlongTrajectory
}

enum MovementState
{
    Arrive,
    Patrol,
    Attack,
    Death
}


public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform _mainCameraTransform;
    [Header("-----Movement Parameters-----")]
    [SerializeField] private float radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private float speed;
    [Header("------Depth Parameters-------")]
    [SerializeField] private bool _isDepthEnabled;
    [SerializeField] private float _depthAmplitude;
    [Header("------------Noise------------")]
    [SerializeField] private bool _isNoiseEnabled;
    [SerializeField] private NoiseType _noiseType;
    [SerializeField] private float _noiseAmplitude;
    [SerializeField] private float _noiseFrequency;
    [Header("-------Enter Animation-------")]
    [SerializeField] private float _enterAnimationDuration;
    [Header("----------Debugging----------")]
    [SerializeField] private float _debugTrailGizmoDelay;
    
    private Vector3 _prevPosition = Vector3.zero;
    private Vector3 _newPosition = Vector3.zero;
    private float _phase = 0;
    private Vector3 _cameraDirection;
    private Vector3 _noiseValue = Vector3.zero;
    
    // Enter Animation
    private Vector3 _startPos;
    private Vector3 _enterPhasePosition;
    private float _prevTime;
    
    private float _offsetAngle;

    private void Start()
    {
        _startPos = new Vector3(-1.8f, -2.5f, 0f);
        _prevTime = Time.time;

        // Calculate an offset Angle
        float a = Mathf.Abs(transform.position.x);
        a = a;
        float d = Vector3.Magnitude(transform.position);
        float r = radius;

        _offsetAngle = Mathf.PI - Mathf.Acos(r / d) - Mathf.Acos(a / d);
        
        Debug.DrawLine(transform.position, Vector3.zero, Color.red, 10f);
        Debug.DrawLine(Vector3.zero, new Vector3(transform.position.x, 0, 0), Color.red, 10f);
        
        // Build a vector r
        Vector3 rVector;
        rVector.x = Mathf.Cos(-_offsetAngle);
        rVector.y = Mathf.Sin(-_offsetAngle);
        rVector.z = 0;
        rVector = rVector.normalized * radius;
        Debug.DrawLine(Vector3.zero, rVector, Color.green, 10f);
        
        Debug.Log(r/d);
        Debug.Log(a/d);
        
        Debug.DrawLine(transform.position, rVector, Color.blue, 10f);
        
        
       


    }

    // Update is called once per frame
    void Update()
    {
        // Circle Movement
        _prevPosition = transform.position;
        _phase = Time.time * speed;

        float xOffset = Mathf.Cos(-_offsetAngle);
        float yOffset = Mathf.Sin(-_offsetAngle);
        
        _newPosition = Vector3.zero;
        _newPosition.x = Mathf.Cos(_phase - xOffset) * radius;
        _newPosition.y = Mathf.Sin(_phase - xOffset) * radius * _verticalAmplitude;
       
        // Adding Noise
        if (_isNoiseEnabled)
        {
            _noiseValue.x = Mathf.PerlinNoise(_noiseFrequency * Time.time, 0) * 2 - 1;
            _noiseValue.y = Mathf.PerlinNoise(0, _noiseFrequency * Time.time) * 2 - 1;

            if (_noiseType == NoiseType.WorldSpace)
            {
                _newPosition += _noiseValue * _noiseAmplitude;
            }
            else if (_noiseType == NoiseType.AlongTrajectory)
            {
                _newPosition += transform.position.normalized * (_noiseValue.x * _noiseAmplitude);
            }
        }
        
        // Enter animation
        // float enterAnimationPhase = (Time.time - _prevTime) / _enterAnimationDuration; 
        //
        // Debug.Log(enterAnimationPhase);
        //
        // if(enterAnimationPhase < 1f)
        // {
        //     _enterPhasePosition = Vector3.Lerp(_startPos, _newPosition, enterAnimationPhase);
        //     transform.position = _enterPhasePosition;
        // }
        // else
        // {
        //     transform.position = _newPosition;            
        // }

        
        // Adding Depth 
        if (_isDepthEnabled)
        {
            _cameraDirection = (_mainCameraTransform.position - transform.position).normalized;
            _newPosition += _cameraDirection * (_depthAmplitude * Mathf.Sin(_phase));    
        }
        
        transform.position = _newPosition;
        
        //Debug.DrawLine(_prevPosition, transform.position, Color.cyan, _debugTrailGizmoDelay);
        //Debug.DrawLine(_mainCameraTransform.position, transform.position, Color.yellow, 0.01f);
        
    }

}
