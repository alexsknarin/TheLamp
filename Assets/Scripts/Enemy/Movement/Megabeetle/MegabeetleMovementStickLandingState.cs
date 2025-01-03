using UnityEngine;

public class MegabeetleMovementStickLandingState: EnemyMovementBaseState
{
    private readonly float NEUTRAL_DISTANCE = 0.44f;
    private float _duration = .491f;
    private float _phase;
    private float _localTime;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _startDistance;
    
    public MegabeetleMovementStickLandingState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }

    public override EnemyState State => EnemyState.StickLanding;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _startDistance = currentPosition.magnitude; 
        _localTime = 0;
        _phase = 0;
        _startPosition = currentPosition;
        _endPosition = currentPosition.normalized * NEUTRAL_DISTANCE;
        Position = currentPosition;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        _phase  = _localTime / _duration;
        Position = Vector3.Lerp(_startPosition, _endPosition, _phase);
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        if (_phase > 1)
        {
            _owner.SwitchState();
        }
    }
}