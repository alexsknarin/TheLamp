    using UnityEngine;

public class EnemyMovementEnterState: EnemyMovementBaseState
{
    private Vector3 _endPos = Vector3.zero;
    private Vector3 _enterDirection;
    
    public EnemyMovementEnterState(EnemyBaseMovement owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    } 


    public override EnemyStates State
    {
        get => EnemyStates.Enter;
        protected set  { }
    }
   
    public override void EnterState(Vector3 currentPosition, int sideDirection)
    {
        _sideDirection = sideDirection;
        float xProjectionLength = Mathf.Abs(currentPosition.x);
        float enterDirectionLength = Vector3.Magnitude(currentPosition);
        float r = _radius * _verticalAmplitude;
        
        float patrolStartOffsetAngle = Mathf.PI - Mathf.Acos(r / enterDirectionLength) - Mathf.Acos(xProjectionLength / enterDirectionLength);
        
        _endPos.x = Mathf.Cos(-patrolStartOffsetAngle) * _sideDirection;
        _endPos.y = Mathf.Sin(-patrolStartOffsetAngle);
        _endPos = _endPos.normalized * r;
       
        _enterDirection = (_endPos - currentPosition).normalized;
        Position = currentPosition;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
    }
    
    public override void CheckForStateChange()
    {
        if(Position.x*_sideDirection > Mathf.Abs(_endPos.x))
        {
            _owner.SwitchState();
        }
    }

    public override void ExitState()
    {
    }
    


}