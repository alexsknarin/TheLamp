using UnityEngine;

public enum LampMotionState
{
    Idle,
    Swing,
    Force
}

public class LampMovement : MonoBehaviour, IInitializable
{
    [Header("|---- Force ----|")]
    [SerializeField] private float _forceMaxMagnitude;
    [Header("|---- Swing ----|")]
    [SerializeField] private float _swingAmplitude;
    [SerializeField] private float _swingFrequency;
    [SerializeField] private float _swingAttenuationDuration;
    [SerializeField] private Vector3 _swingAimPoint;
    private float _swingDurationNormalized;
    [SerializeField] private AnimationCurve _swingAttenuationCurve;
    [SerializeField] private LampMotionState _lampMotionState = LampMotionState.Idle;
   
    // Force
    private float _forcePhase;
    // Swing
    private float _swingShift;
    private float _currentCenter;
    private int _velocityDirection;
    private float _localTime;
    
    private float _deviationAngle = 0;
    private Vector3 _prevPos;
    private Vector3 _newPos;
    
    private float _forceDirection;

    public void Initialize()
    {
        _newPos = transform.position;
        _deviationAngle = 0;
    }
    
    public void AddForce(float force)
    {
        if(_lampMotionState == LampMotionState.Idle)
        {
            _forceDirection = force;
            _currentCenter = _newPos.x;
            _localTime = 0;
            
            _lampMotionState = LampMotionState.Force;
            _forcePhase = 0;
            _swingDurationNormalized = _swingAttenuationDuration;
        }
        else if (_velocityDirection == (int)Mathf.Sign(force))
        {
            _forceDirection = force;
            _currentCenter = _newPos.x;
            _localTime = 0;
            _lampMotionState = LampMotionState.Force;
            _forcePhase = 0;
        }
        else
        {
            StartSwing();
        }
    }
    
    private void PerformApplyForce()
    {
        _forcePhase = Mathf.Sin((_localTime) * _swingFrequency );
        float prevX = _newPos.x;
        _newPos.x = _currentCenter + _forceDirection * _forcePhase * _forceMaxMagnitude;
        if (float.IsNaN(_newPos.x))
        {
            _newPos.x = prevX;
        }
        CompensateRotation(_newPos);
        _localTime += Time.deltaTime;
        if (_forcePhase > 0.95f)
        {
            StartSwing();
        }
    }
    
    private void StartSwing()
    {
        _currentCenter = 0;
        _swingAmplitude = Mathf.Abs(_newPos.x);
        _swingDurationNormalized = (_swingAttenuationDuration * _swingAmplitude) / _forceMaxMagnitude;
        _localTime = 0;
        
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

    private void PerformSwing()
    {
        float attenuationPhase = _localTime / _swingDurationNormalized;
        float prevX = _newPos.x;
        _newPos.x = Mathf.Sin((_localTime + _swingShift) * _swingFrequency) * _swingAmplitude * _swingAttenuationCurve.Evaluate(attenuationPhase);
        if (float.IsNaN(_newPos.x))
        {
            _newPos.x = prevX;
        }
        CompensateRotation(_newPos);
        if (attenuationPhase > 1)
        {
            _lampMotionState = LampMotionState.Idle;
        }
        _localTime += Time.deltaTime;
    }

    private void CompensateRotation(Vector3 pos)
    {
        Vector3 aimDirection = _swingAimPoint - pos;
        _deviationAngle = Mathf.Acos(Vector3.Dot(aimDirection.normalized, Vector3.up));
        if (pos.x > 0)
        {
            _deviationAngle *= -1;
        }
    }

    private void PerformIdle()
    {
        _newPos.x = 0f;
        _deviationAngle = 0;
    }
    
    void Update()
    {
        _prevPos = _newPos;

        switch (_lampMotionState)
        {
            case LampMotionState.Idle:
                PerformIdle();
                break;
            case LampMotionState.Swing:
                PerformSwing();
                break;
            case LampMotionState.Force:
                PerformApplyForce();
                break;
        }
        
        _velocityDirection = (int)Mathf.Sign(_newPos.x - _prevPos.x);
        transform.position = _newPos;
        transform.localRotation = Quaternion.Euler(0, 0, -_deviationAngle * Mathf.Rad2Deg);
    }
}
