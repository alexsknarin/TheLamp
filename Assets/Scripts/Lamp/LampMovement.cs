using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LampMotionState
{
    Idle,
    Swing,
    Force
}

public class LampMovement : MonoBehaviour
{
    [Header("|---- Force ----|")]
    [SerializeField] private float _forceMagnitude;
    
    [Header("|---- Swing ----|")]
    [SerializeField] private float _swingAmplitude;
    [SerializeField] private float _swingFrequency;
    [SerializeField] private float _swingAttenuationDuration;
    [SerializeField] private AnimationCurve _swingAttenuationCurve;
    
    [SerializeField] private LampMotionState _lampMotionState = LampMotionState.Idle;
    
    
    // Force
    private bool _isForceApplied = false;
    private float _forcePhase;
    private float _prevForcePhase;
    
    // Swing
    private float _swingShift;
    
    private Vector3 _newPos;
    private float _currentCenter;
    private float _prevTime;
    
    
    
    
    
    private 
    
    // TMP
    Camera _camera;

    private int _forceDirection;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        _newPos = transform.position;
        _prevTime = Time.time;
    }
    
    private void AddForce(int direction)
    {
        _isForceApplied = true;
        _forceDirection = direction;
        _currentCenter = _newPos.x;
        _prevTime = Time.time;
        _lampMotionState = LampMotionState.Force;
        _forcePhase = 0;
    }
    
    private void ApplyForce()
    {
        if (_isForceApplied)
        {
            _prevForcePhase = _forcePhase;
            
            _forcePhase = Mathf.Sin((Time.time - _prevTime) * _swingFrequency );
            
            if (_forcePhase > 0.1f && _forcePhase < _prevForcePhase)
            {
                StartSwing();
                
            }
            _newPos.x = _currentCenter + _forceDirection * _forcePhase * _forceMagnitude;
        }
    }
    
    private void StartSwing()
    {
        _currentCenter = 0;
        _swingAmplitude = Mathf.Abs(_newPos.x);
        _prevTime = Time.time;
        
        if(_newPos.x > 0)
        {
            _swingShift = Mathf.PI * 0.25f;
        }
        else
        {
            _swingShift = Mathf.PI * 0.75f;
        }
        
        _lampMotionState = LampMotionState.Swing;
    }

    private void ApplySwing()
    {
        float attenuationPhase = (Time.time - _prevTime) / _swingAttenuationDuration;
        _newPos.x = Mathf.Sin((Time.time - _prevTime + _swingShift) * _swingFrequency) * _swingAmplitude * _swingAttenuationCurve.Evaluate(attenuationPhase);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x > 540)
            {
                AddForce(-1);
            }
            else
            {
                AddForce(1);
            }
        }

        switch (_lampMotionState)
        {
            case LampMotionState.Idle:
                break;
            case LampMotionState.Swing:
                ApplySwing();
                break;
            case LampMotionState.Force:
                ApplyForce();
                break;
        }

        transform.position = _newPos;

        
        

        // transform.position = _newPos;
    }
}
