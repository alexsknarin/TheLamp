using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpiderMovement : EnemyMovement
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    private int _sideDirection;
    private float _xCenter = 1.12f;

    // Movement States
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private SpiderMovementEnterState _enterState;
    private SpiderMovementPatrolState _patrolState;
    private SpiderMovementPreAttackState _preAttackState;
    private SpiderMovementAttackState _attackState;
    private SpiderMovementReturnState _returnState;
    private FlyMovementDeathState _deathState;
    
    private Vector3 _prevPosition2d; //Debug
    private Vector3 _prevPosSmooth; //Debug
    private Vector3 _position2d;
    
   
    
    // State parameters
    private bool _isDead = false;
    private bool _isCollided = false;
    
    // Debug
    [SerializeField] private EnemyStates _stateDebug;
    
    public override void Initialize()
    {
        _sideDirection = RandomDirection.Generate();
        _xCenter *= _sideDirection;
        
        _movementStateMachine = new EnemyMovementStateMachine();
        
        _enterState = new SpiderMovementEnterState(this, _speed, _xCenter, 0);
        _patrolState  = new SpiderMovementPatrolState(this, _speed, _xCenter, 0);
        _preAttackState = new SpiderMovementPreAttackState(this, _speed, _xCenter, 0);
        _attackState = new SpiderMovementAttackState(this, _speed, _xCenter, 0);
        _returnState = new SpiderMovementReturnState(this, _speed, _xCenter, 0);
        _deathState = new FlyMovementDeathState(this, _speed, _xCenter, 0);
        
        Spawn();
        
        _currentState = _enterState;
        
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        _position2d.x = _xCenter;
        transform.position = _position2d;
    }
    
    private void Spawn()
    {
        _position2d = GenerateSpawnPosition(-_sideDirection);
    }
    
    private Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = Vector3.up * 5f;
        spawnPosition.x = _xCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }
    
    public override void TriggerFall()
    {
        if(_currentState.State == EnemyStates.Attack)
        {
            _isCollided = true;
            SwitchState();
        }
    }

    public override void TriggerDeath()
    {
        if(_currentState.State != EnemyStates.Death)
        {
            _isDead = true;
            SwitchState();
        }
    }

    public override void TriggerAttack()
    {
        SwitchState();
    }

    public override void TriggerStick()
    {
    }
    
    public override void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState;
        switch (_currentState.State)
        {
            case EnemyStates.Enter:
                newState = _patrolState;
                break;
            case EnemyStates.Patrol:
                OnPreAttackStartInvoke();
                newState = _preAttackState;
                break;
            case EnemyStates.PreAttack:
                OnPreAttackEndInvoke();
                newState = _attackState;
                break;
            case EnemyStates.Attack:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                else
                {
                    newState = _returnState;
                    _isCollided = false;
                }
                break;
            case EnemyStates.Return:
                newState = _patrolState;
                break;
            case EnemyStates.Death:
                OnEnemyDeactivatedInvoke();
                break;
        }

        _currentState = newState;
        State = _currentState.State;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        OnStateChangeInvoke();
    }

    private void Update()
    {   
        _prevPosition2d = _position2d;
        
        // Debug Only
        _prevPosSmooth = transform.position;
       
        _movementStateMachine.Execute(_position2d);
        _position2d = _currentState.Position;
       
        _movementStateMachine.CheckForStateChange();
        
        transform.position = _position2d;
        
        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d-_prevPosition2d).normalized*0.02f, Color.cyan, 5f);
        Debug.DrawLine(_prevPosSmooth, _prevPosSmooth + (transform.position-_prevPosSmooth).normalized*0.02f, Color.yellow, 5f);
        
        _stateDebug = _currentState.State;
        
        
        
        if(Input.GetKeyDown(KeyCode.F))
        {
            if (_currentState.State == EnemyStates.Patrol)
            {
                SwitchState();
            }
        }
        
    }
}
