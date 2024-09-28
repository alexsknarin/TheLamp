using System;
using UnityEngine;

public class DragonflySpider : MonoBehaviour
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

    private Vector3 _currentEndPosition;
    private Vector3 _startPosition;
    private bool _isStartPlaying = false;
    private float _localTime = 0f;
    
    private bool _isAttackPlaying = false;
    private Vector3 _attackDirection;
    
    private bool _isFallPlaying = false;
    private float _bounceSpeed = 0f;

    public void PlayStartAnimation(int direction)
    {
        _isStartPlaying = true;
        _localTime = 0f;
        
        _currentEndPosition = _endPosition;
        _currentEndPosition.x *= direction;
        _startPosition = _currentEndPosition + Vector3.up * _startHeight;
        transform.position = _startPosition;
    }

    public void PlayAttackAnimation()
    {
        _isAttackPlaying = true;
        _attackDirection = -transform.position.normalized;
    }
    
    private void PlayFallAnimation()
    {
        _isFallPlaying = true;
        _localTime = 0f;
        _startPosition = transform.localPosition;
        _bounceSpeed = _attackSpeed/10f;
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
            transform.position += _attackDirection * (_attackSpeed * Time.deltaTime);
        }
        
        if (_isFallPlaying)
        {
            float phase = _localTime / _fallDuration;
            
            if (phase > 1)
            {
                _isFallPlaying = false;
                return;
            }
            Vector3 pos = transform.localPosition;
            pos += -_attackDirection * (_bounceSpeed * Time.deltaTime);
            pos.y = _startPosition.y + _fallAnimCurve.Evaluate(phase);
            transform.localPosition = pos;
            
            _localTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isAttackPlaying)
        {
            _isAttackPlaying = false;
            PlayFallAnimation();
        }
    }
}
