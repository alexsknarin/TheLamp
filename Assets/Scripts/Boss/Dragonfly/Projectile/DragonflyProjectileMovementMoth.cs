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
    
    public event Action OnFallEnded;
    
    
    private bool _isAttacking = false;
    private bool _isFalling = false;
    private Vector3 _attackDirection;
    private float _currentAcceeleration = 0f;
    private Vector3 _sideGoal;
    
    // Debug
    private Vector3 _previousPosition;
    private Vector3 _previousPositionRaw;
    
    

    public void Initialize(Vector3 startPosition)
    {
        transform.position = startPosition;
    }

    public void TriggerAttack()
    {
        _isFalling = false;
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
            _isFalling = true;
            _currentAcceeleration = 0f;    
        }
    }

    void Update()
    {
        if (_isAttacking)
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
        
        if (_isFalling)
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
                OnFallEnded?.Invoke();
            }
        }
    }

}
