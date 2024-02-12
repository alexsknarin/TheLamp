using UnityEngine;

public class EnemyMovementEnterState: EnemyMovementBaseState
{
    public EnemyMovementEnterState(EnemyBaseMovement owner, int sideDirection, float speed, float radius, float verticalAmplitude) : base()
    {
        _sideDirection = sideDirection;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    private Vector3 _startPos;
    private Vector3 _endPos = Vector3.zero;
    private Vector3 _enterDirection;
 
    // for Scriprable Object UI
    private float _spawnAreaSize = 0.5f;
    private Vector3 _spawnAreaCenter;

    public override EnemyStates State
    {
        get => EnemyStates.Enter;
        protected set  { }
    }
   
    public override void EnterState(Vector3 currentPosition)
    {
        _spawnAreaCenter = new Vector3(2.6f, -2.6f, 0); // Hardcoded for now - TODO: remove new Vector3 and replace with a serialized field
 
        _startPos = GenerateSpawnPosition(-_sideDirection);
        float xProjectionLength = Mathf.Abs(_startPos.x);
        float enterDirectionLength = Vector3.Magnitude(_startPos);
        float r = _radius * _verticalAmplitude;
        
        float patrolStartOffsetAngle = Mathf.PI - Mathf.Acos(r / enterDirectionLength) - Mathf.Acos(xProjectionLength / enterDirectionLength);
        Debug.Log(patrolStartOffsetAngle);
        
        _endPos.x = Mathf.Cos(-patrolStartOffsetAngle) * _sideDirection;
        _endPos.y = Mathf.Sin(-patrolStartOffsetAngle);
        _endPos = _endPos.normalized * r;
       
        _enterDirection = (_endPos - _startPos).normalized;
        Position = _startPos;
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
    
    private Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = (Vector3)(Random.insideUnitCircle * _spawnAreaSize) + _spawnAreaCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }


}