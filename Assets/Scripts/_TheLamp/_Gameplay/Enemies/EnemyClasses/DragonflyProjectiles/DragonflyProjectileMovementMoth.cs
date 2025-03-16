using System;
using UnityEngine;

public class DragonflyProjectileMovementMoth : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _bounceSpeed = 1f;
    [SerializeField] private float _fallSpeed = 1.5f;
    [SerializeField] private float _fallAcceleraion = 1.6f;
    [SerializeField] private float _noiseFrequency = 1.0f;
    [SerializeField] private float _noiseAmplitude = 1.0f;
    [SerializeField] private float _startTransitionDistance = 2.0f;
    [SerializeField] private float _fleeTurnDuration = 0.5f;
    [SerializeField] private Vector3 _fleeGoalBase;
    private bool _isAttacking = false;
    private bool _isFalling = false;
    private bool _isFleeing = false;
    private Vector3 _attackDirection;
    private float _currentAcceeleration = 0f;
    private Vector3 _sideGoal;
    private Vector3 _fleeDirection;
    private Vector3 _fleeGoal;
    private float _localTime = 0f;
    // Debug
    private Vector3 _previousPosition;
    private Vector3 _previousPositionRaw;
    public event Action FallEnded;


    public void Initialize(Vector3 startPosition)
    {
        transform.position = startPosition;
    }

    public void TriggerAttack()
    {
        _isFalling = false;
        _isFleeing = false;
        _isAttacking = true;
        _previousPositionRaw = transform.position;
        
        _sideGoal = transform.position;
        _sideGoal.z = 0;
        _sideGoal.Normalize();
        _sideGoal *= 0.95f;
       
        _attackDirection = (_sideGoal - transform.position).normalized;
    }
    
    public void TriggerFall()
    {
        if (!_isFalling)
        {
            _isAttacking = false;
            _isFleeing = false;
            _isFalling = true;
            _currentAcceeleration = 0f;    
        }
    }

    public void TriggerGameOver()
    {
        if (_isAttacking)
        {
            _fleeGoal = _fleeGoalBase;
            if (transform.position.x < 0)
            {
                _fleeGoal.x = -_fleeGoal.x;
            }
            _fleeDirection = (_fleeGoal - transform.position).normalized;
            
            _isFalling = false;
            _isAttacking = false;
            _isFleeing = true;
        }
    }

    private void Update()
    {
        if (_isAttacking)
        {
            Attack();
        }
        
        if (_isFalling)
        {
            Fall();
        }
        
        if (_isFleeing)
        {
            Flee();
        }
    }

    private void Fall()
    {
        Vector3 position = transform.position;
        // Bounce
        position -= _attackDirection * (_bounceSpeed * Time.deltaTime);
        // Fall
        _currentAcceeleration += _fallAcceleraion * Time.deltaTime;
        position += Vector3.down * (_fallSpeed * Time.deltaTime * _currentAcceeleration);
        transform.position = position;
            
        if (transform.position.y < -10f)
        {
            _isFalling = false;
            _isAttacking = false;
            transform.position = new Vector3(0, -2.294306f, -3.276608f);
            FallEnded?.Invoke();
        }
    }

    private void Attack()
    {
        float distance = Mathf.Abs(transform.position.z);
        if (distance < _startTransitionDistance)
        {
            float phase = Mathf.Pow(1 - distance / _startTransitionDistance, 2.5f);
            Vector3 midGoalPosition = Vector3.Lerp(_sideGoal, Vector3.zero, phase);
            _attackDirection = (midGoalPosition - transform.position).normalized;
        }

        _previousPosition = transform.position;
        Vector3 position = _previousPositionRaw + _attackDirection * (_speed * Time.deltaTime);
        _previousPositionRaw = position;
        // Add noise
        position.x += (Mathf.PerlinNoise(Time.time * _noiseFrequency, 0) - 0.5f) * 2 * _noiseAmplitude;
        position.y += (Mathf.PerlinNoise(0, Time.time * _noiseFrequency) - 0.5f) * 2 * _noiseAmplitude;
        transform.position = position;
            
        Debug.DrawLine(_previousPosition, transform.position, Color.cyan, 5f);
    }
    
    private void Flee()
    {
        float phase = _localTime / _fleeTurnDuration;
        if (phase > 1f)
        {
            phase = 1f;
        }
        Vector3 direction = Vector3.Slerp(_attackDirection, _fleeDirection, phase);
        
        _previousPosition = transform.position;
        Vector3 position = _previousPositionRaw + direction * (_speed * Time.deltaTime);
        _previousPositionRaw = position;
        // Add noise
        position.x += (Mathf.PerlinNoise(Time.time * _noiseFrequency, 0) - 0.5f) * 2 * _noiseAmplitude;
        position.y += (Mathf.PerlinNoise(0, Time.time * _noiseFrequency) - 0.5f) * 2 * _noiseAmplitude;
        transform.position = position;
            
        Debug.DrawLine(_previousPosition, transform.position, Color.cyan, 5f);
    }
}
