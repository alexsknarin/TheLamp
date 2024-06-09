using UnityEngine;

public class MegabeetleMovementStickState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Stick;
    private float _duration = 1.25f;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public MegabeetleMovementStickState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        Position = currentPosition.normalized * 0.44f;
        _localTime = 0;
        _phase = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        _phase  = _localTime / _duration;
        Position = currentPosition;
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