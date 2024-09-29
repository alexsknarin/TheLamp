using System;
using UnityEngine;

public class DragonflySwarmMoth : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _bounceSpeed = 5f;
    [SerializeField] private float _fallSpeed = 5f;
    [SerializeField] private float _fallAcceleraion = 5f;
    
    
    private bool _isAttacking = false;
    private bool _isFalling = false;
    private Vector3 _attackDirection;
    private float _currentAcceeleration = 0f;
    
    public void PlayAttack(Vector3 startPosition)
    {
        _isFalling = false;
        _isAttacking = true;
        transform.position = startPosition;
        _attackDirection = - startPosition.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isAttacking)
        {
            transform.position += _attackDirection * (_speed * Time.deltaTime);
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
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _isAttacking = false;
        _isFalling = true;
        _currentAcceeleration = 0f;
    }
}
