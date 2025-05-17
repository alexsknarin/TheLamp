using System;
using _GAME.Scripts.Enemies.Megamothling;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

public class MegamothlingBodyRotationHandler : MonoBehaviour, IInitializable
{
    [SerializeField] private MegamothlingMovement _movement;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private float _smoothTime = 0.5f;
    [SerializeField] private float _attackSmoothTime = 0.01f;
    [SerializeField] private float _attackSmoothTransitionDuration = 0.4f;
    [SerializeField] private float _fallSmoothTime = 0.5f;
    [SerializeField] private float _fallSmoothTransitionDuration = 0.4f;
    
    private Vector3 _previousSmoothPosition = Vector3.zero;
    private Vector3 _currentSmoothPosition = Vector3.zero;
    private Vector3 _currentForwardVelocity;
    private Vector3 _smoothVelocity;
    
    private bool _isAttacking;
    private bool _isFalling;
    
    private float _smoothTransitionLocalTime;

    public void Initialize()
    {
        _movement.AttackStarted += OnAttackStarted;
        _movement.AttackEnded += OnAttackEnded;
    }

    private void OnDestroy()
    {
        _movement.AttackStarted -= OnAttackStarted;
        _movement.AttackEnded -= OnAttackEnded;
    }


    void Update()
    {
        float currentSmoothTime = _smoothTime;
        
        if (_isAttacking)
        {
            float smoothTransitionPhase = _smoothTransitionLocalTime / _attackSmoothTransitionDuration;
            if (smoothTransitionPhase > 1f)
            {
                currentSmoothTime = _attackSmoothTime;
            }
            else
            {
                currentSmoothTime = Mathf.Lerp(_smoothTime, _attackSmoothTime, smoothTransitionPhase);
                _smoothTransitionLocalTime += Time.deltaTime;
            }
        }
        
        if (_isFalling)
        {
            float smoothTransitionPhase = _smoothTransitionLocalTime / _fallSmoothTransitionDuration;
            if (smoothTransitionPhase > 1f)
            {
                _isFalling = false;
            }
            else
            {
                currentSmoothTime = Mathf.Pow(currentSmoothTime, 0.2f);
                
                currentSmoothTime = Mathf.Lerp(_fallSmoothTime, _smoothTime, smoothTransitionPhase);
                _smoothTransitionLocalTime += Time.deltaTime;
            }
        }
        
        
        
        _currentSmoothPosition =
            Vector3.SmoothDamp(_currentSmoothPosition, transform.position, ref _smoothVelocity, currentSmoothTime);
        
        Debug.DrawLine(_previousSmoothPosition, _currentSmoothPosition, Color.red, 5f);
        
        
        _currentForwardVelocity = (_currentSmoothPosition - _previousSmoothPosition).normalized;


        Vector3 up = Vector3.up;
        _bodyTransform.LookAt(_bodyTransform.position + _currentForwardVelocity, up);
        
        
        _previousSmoothPosition = _currentSmoothPosition;
    }

    private void OnAttackStarted()
    {
        _isAttacking = true;
        _smoothTransitionLocalTime = 0f;
    }

    private void OnAttackEnded()
    {
        _isAttacking = false;
        _isFalling = true;
    }
}
