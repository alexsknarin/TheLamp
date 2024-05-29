using UnityEngine;

public class SpiderMovementEnterState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Enter;
    private Vector3 _hangingPoint;
    private float _localTime;
    
    public SpiderMovementEnterState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
        
        _hangingPoint = Vector3.up * 5f;
        _hangingPoint.x = radius;
    } 
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _hangingPoint.x = Mathf.Abs(_hangingPoint.x) * sideDirection;
        Position = currentPosition;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 newPosition = currentPosition;
        float phase = Mathf.Pow(currentPosition.y / 5f, 0.5f);
        newPosition.y = currentPosition.y - (_speed * phase * Time.deltaTime);
        
        float swing = Mathf.Sin(_localTime * 4.1f) * 0.1f * phase + _hangingPoint.x;
        newPosition.x = swing; 

        Position = newPosition;
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        if(Position.y < 0.005f)
        {
            _owner.SwitchState();
        }
    }

    public override void ExitState()
    {
        Position = Vector3.right * _hangingPoint.x;
    }
}