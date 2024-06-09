using UnityEngine;

public class MegabeetleMovementStickPreAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.StickPreAttack;
    private float _duration = 3f;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    
    public MegabeetleMovementStickPreAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        Position = currentPosition;
        
        _startPosition = currentPosition;
        _endPosition = currentPosition.normalized * 0.66f;
        _localTime = 0;
        _phase = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        _phase  = _localTime / _duration;
        Position = Vector3.Lerp(_startPosition, _endPosition, Mathf.Pow(_phase, 0.45f));
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