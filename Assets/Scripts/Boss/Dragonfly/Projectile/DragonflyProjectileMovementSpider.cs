using System;
using UnityEngine;

public class DragonflyProjectileMovementSpider : MonoBehaviour
{
    [SerializeField] private Vector3 _endPosition;
    [SerializeField] private float _startHeight = 2.4f;
    [SerializeField] private AnimationCurve _startAnimCurve;
    [SerializeField] private float _startDuration = 2f;
    [Header("Attack")]
    [SerializeField] private float _attackSpeed = 5f;
    [Header("Fall")]
    [SerializeField] private float _fallDuration = 2f;
    [SerializeField] private AnimationCurve _fallAnimCurve;
    
    public event Action OnEnterAnimationEnd;
    public event Action OnFallEnded;

    private Vector3 _currentEndPosition;
    private Vector3 _startPosition;
    private bool _isStartPlaying = false;
    private float _localTime = 0f;
    
    private bool _isAttackPlaying = false;
    private Vector3 _attackDirection;
    
    private bool _isFallPlaying = false;
    private float _bounceSpeed = 0f;
    private Vector3 _sideGoal;
    private float _startTransitionDistance; 
    
    
    // Debug 
    Vector3 _previousPosition;
    
   
    public void Initialize(int direction)
    {
        _isStartPlaying = true;
        _isFallPlaying = false;
        _localTime = 0f;
        
        _currentEndPosition = _endPosition;
        _currentEndPosition.x *= direction;
        _startPosition = _currentEndPosition + Vector3.up * _startHeight;
        transform.position = _startPosition;
    }

    public void TriggerAttack()
    {
        _isAttackPlaying = true;
        _attackDirection = -transform.position.normalized;
        
        _sideGoal = transform.position;
        _sideGoal.z = 0;
        _sideGoal.Normalize();
        _sideGoal *= 0.95f;

        _startTransitionDistance = Mathf.Abs(transform.position.z);
        
        _attackDirection = (_sideGoal - transform.position).normalized;
    }
    
    public void TriggerFall()
    {
        if (!_isFallPlaying)
        {
            _isAttackPlaying = false;
            _isFallPlaying = true;
            _localTime = 0f;
            _startPosition = transform.localPosition;
            _bounceSpeed = _attackSpeed/10f;  
        }
    }
    
    private void Update()
    {
        if (_isStartPlaying)
        {
            float phase = _localTime / _startDuration;
            if (phase > 1)
            {
                _isStartPlaying = false;
                OnEnterAnimationEnd?.Invoke();
                return;
            }
            transform.position = Vector3.Lerp(_startPosition, _currentEndPosition, _startAnimCurve.Evaluate(phase));
            _localTime += Time.deltaTime;
        }

        if (_isAttackPlaying)
        {
            float distance = Mathf.Abs(transform.position.z);
            float phase = Mathf.Pow(1 - distance / _startTransitionDistance, 2.5f);
            Vector3 midGoalPosition = Vector3.Lerp(_sideGoal, Vector3.zero, phase);
            _attackDirection = (midGoalPosition - transform.position).normalized;
            
            _previousPosition = transform.position;
            transform.position += _attackDirection * (_attackSpeed * Time.deltaTime);
            
            Debug.DrawLine(_previousPosition, transform.position, Color.cyan, 5f);
        }
        
        if (_isFallPlaying)
        {
            float phase = _localTime / _fallDuration;
            
            if (phase > 1)
            {
                _isFallPlaying = false;
                OnFallEnded?.Invoke();
                return;
            }
            Vector3 pos = transform.localPosition;
            pos += -_attackDirection * (_bounceSpeed * Time.deltaTime);
            pos.y = _startPosition.y + _fallAnimCurve.Evaluate(phase);
            transform.localPosition = pos;
            _localTime += Time.deltaTime;
        }
    }
}
