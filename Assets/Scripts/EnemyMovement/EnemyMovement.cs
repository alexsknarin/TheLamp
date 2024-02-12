using System;
using UnityEngine;
using Random = UnityEngine.Random;

enum NoiseType
{
    WorldSpace,
    AlongTrajectory
}

enum MovementState
{
    Enter,
    Patrol,
    Attack,
    Fall,
    Death
}


public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform _mainCameraTransform;
    [Header("-----Movement Parameters-----")]
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private float speed;
    
    [Header("------Depth Parameters-------")]
    [SerializeField] private bool _isDepthEnabled;
    [SerializeField] private float _depthAmplitude;
    
    [Header("------------Noise------------")]
    [SerializeField] private bool _isNoiseEnabled;
    [SerializeField] private NoiseType _noiseType;
    [SerializeField] private float _noiseAmplitude;
    [SerializeField] private float _noiseFrequency;
    
    [Header("------------Spawn------------")]
    [SerializeField] private Vector3 _spawnAreaCenter;
    [SerializeField] private float _spawnAreaSize;
    
    // [Header("-------Enter Move State-------")]
    // [SerializeField] private float _enterAnimationDuration;
    
    [Header("------Patrol Move State------")]
    [SerializeField] private float _verticalAdaptDuration;
    
    [Header("------Attack Move State------")]
    [SerializeField] private float _attackPrepareDuration;
    [SerializeField] private float _attackPrepareDistance; 
    
    [Header("----------Debugging----------")]
    [SerializeField] private float _debugTrailGizmoDelay;
    
    private Rigidbody2D _rigidbody2D;
    
    private MovementState _movementState;
    
    
    private Vector3 _prevPosition = Vector3.zero;
    private Vector3 _newPosition = Vector3.zero;
    private float _phase = 0;
    private Vector3 _cameraDirection;
    private Vector3 _noiseValue = Vector3.zero;
    
    // Enter Move State
    private int _movementDirection;
    private Vector3 _startPos;
    private Vector3 _enterPhasePosition;
    private Vector3 _enterMovementDirection;
    private float _prevTime;
    private float _enterTimeOffset = 0f;
    private Vector3 _patrolStartPos = Vector3.zero;
    
    // patrol Move State
    private float _patrolStartOffsetAngle;
    private float _finalXRadius;
    private bool _isAttackPreparationStarted = false;
    private float _attackPreparePhase = 0f;
    private float _attackPreparePrevTime;
    
    // Attack Move State
    private Vector3 _attackDirection;

    private int GetRandomDirection()
    {
        return Random.Range(0, 2) * 2 - 1;
    }
    
    private Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = (Vector3)(Random.insideUnitCircle * _spawnAreaSize) + _spawnAreaCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }
    
    private void Init()
    {
        _movementDirection = GetRandomDirection();
        _startPos = GenerateSpawnPosition(_movementDirection);
        transform.position = _startPos;
        
        var position = transform.position;
        float xProjectionLength = Mathf.Abs(position.x);
        float enterDirectionLength = Vector3.Magnitude(position); // TODO: calculate noise to add it into account
        float r = _radius * _verticalAmplitude;

        _patrolStartOffsetAngle = Mathf.PI - Mathf.Acos(r / enterDirectionLength) - Mathf.Acos(xProjectionLength / enterDirectionLength);

        _patrolStartPos.x = -Mathf.Cos(-_patrolStartOffsetAngle) * _movementDirection;
        _patrolStartPos.y = Mathf.Sin(-_patrolStartOffsetAngle);
        
        _patrolStartPos = _patrolStartPos.normalized * r;
        
        _enterMovementDirection = (_patrolStartPos - _startPos).normalized;
        Debug.Log(_enterMovementDirection);
        
        _movementState = MovementState.Enter;
        _prevTime = Time.time;
    }
    
    //
    // TODO: reverse movement direction ???????
    //
    
    private void DrawInitDebugGizmos(Vector3 startPos, float radius, float patrolStartOffsetAngle, Vector3 patrolStartPos)
    {
        Debug.DrawLine(startPos, Vector3.zero, Color.red, 10f);
        Debug.DrawLine(Vector3.zero, new Vector3(startPos.x, 0, 0), Color.red, 10f);
        Debug.DrawLine(Vector3.zero, patrolStartPos, Color.green, 10f);
        Debug.DrawLine(startPos, patrolStartPos, Color.blue, 10f);
    }

    private void EnterMovePerform()
    {
        _prevPosition = transform.position;
        _newPosition = transform.position + _enterMovementDirection * (speed * Time.deltaTime * (Mathf.PI/2));
        transform.position = _newPosition;
        
        Debug.DrawLine(_prevPosition, _prevPosition + _enterMovementDirection*0.01f, Color.cyan, _debugTrailGizmoDelay);

        if(-transform.position.x*_movementDirection > Mathf.Abs(_patrolStartPos.x))
        {
            _enterTimeOffset = Time.time;
            _prevTime = Time.time;
            _movementState = MovementState.Patrol;
        }
    }
    
    private void Start()
    {
        // _rigidbody2D = GetComponent<Rigidbody2D>();
        // Init();
        // DrawInitDebugGizmos(_startPos, _radius, _patrolStartOffsetAngle, _patrolStartPos);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_movementState)
        {
            case MovementState.Enter:
                EnterMovePerform();
                break;
            case MovementState.Patrol:
                PatrolMovePerform();
                break;
            case MovementState.Attack:
                AttackMovePerform();
                break;
            case MovementState.Fall:
                FallMovePerform();
                break;
            case MovementState.Death:
                DeathMovePerform();
                break;
        }
    }

    private void FallMovePerform()
    {
        Debug.Log("I'm FALLING!!!!!");
    }

    private void DeathMovePerform()
    {
        throw new NotImplementedException();
    }

    private void AttackMovePerform()
    {
        _prevPosition = transform.position;
        _newPosition = transform.position + _attackDirection * (speed * Time.deltaTime * (Mathf.PI/2));
        transform.position = _newPosition;
        
        Debug.DrawLine(_prevPosition, _prevPosition + _enterMovementDirection*0.01f, Color.cyan, _debugTrailGizmoDelay);
    }

    private void PatrolMovePerform()
    {
        // Adapt Radius
        float radiusAdaptPhase = (Time.time - _prevTime) / _verticalAdaptDuration;
        if (radiusAdaptPhase > 1f)
        {
            _finalXRadius = _radius;
        }
        else
        {
            _finalXRadius = Mathf.Lerp(_radius * _verticalAmplitude, _radius,  Mathf.SmoothStep(0, 1, radiusAdaptPhase));
        }
        
        _prevPosition = transform.position;
        _phase = -(Time.time - _enterTimeOffset) * speed * _movementDirection;
      
        _newPosition = Vector3.zero;
        float offsetAngleWithDirection;
        if (_movementDirection < 0)
        {
            offsetAngleWithDirection = _patrolStartOffsetAngle;
        }
        else
        {
            offsetAngleWithDirection = -_patrolStartOffsetAngle-Mathf.PI;
        }
       
        _newPosition.x = Mathf.Cos(_phase - offsetAngleWithDirection) * _finalXRadius;               //TODO: X radius Y radius
        _newPosition.y = Mathf.Sin(_phase - offsetAngleWithDirection) * _radius * _verticalAmplitude;
        
        // Adding Depth 
        if (_isDepthEnabled)
        {
            _cameraDirection = (_mainCameraTransform.position - transform.position).normalized;
            _newPosition += _cameraDirection * (_depthAmplitude * Mathf.Sin(_phase - _patrolStartOffsetAngle));
        }
        
        transform.position = _newPosition;
        Vector3 patrolDirection = (_newPosition - _prevPosition).normalized;
        Debug.DrawLine(_prevPosition, _prevPosition + patrolDirection*0.01f, Color.cyan, _debugTrailGizmoDelay);

        
        if (Time.time > 5f)
        {
            if (!_isAttackPreparationStarted)
            {
                _attackPreparePrevTime = Time.time;
                _isAttackPreparationStarted = true;
            }
            else
            {
                // Start Attack Preparation Move
                _attackPreparePhase = (Time.time - _attackPreparePrevTime) / _attackPrepareDuration;
                transform.position = Vector3.Lerp(_newPosition, _newPosition + _newPosition.normalized * _attackPrepareDistance, Mathf.Pow(_attackPreparePhase, 5));
                
                if (_attackPreparePhase > 1f)
                {
                    _movementState = MovementState.Attack;
                    _attackDirection = (Vector3.zero - transform.position).normalized;
                }
            }
        }
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided");
        transform.position += -_attackDirection * 0.15f;
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _movementState = MovementState.Fall;
    }


    private void TMP()
    {
        // Circle Movement
        _prevPosition = transform.position;
        _phase = Time.time * speed;

        float xOffset = Mathf.Cos(-_patrolStartOffsetAngle);
        float yOffset = Mathf.Sin(-_patrolStartOffsetAngle);
        
        _newPosition = Vector3.zero;
        _newPosition.x = Mathf.Cos(_phase - xOffset) * _radius;
        _newPosition.y = Mathf.Sin(_phase - xOffset) * _radius * _verticalAmplitude;
       
        // Adding Noise
        if (_isNoiseEnabled)
        {
            _noiseValue.x = Mathf.PerlinNoise(_noiseFrequency * Time.time, 0) * 2 - 1;
            _noiseValue.y = Mathf.PerlinNoise(0, _noiseFrequency * Time.time) * 2 - 1;

            if (_noiseType == NoiseType.WorldSpace)
            {
                _newPosition += _noiseValue * _noiseAmplitude;
            }
            else if (_noiseType == NoiseType.AlongTrajectory)
            {
                _newPosition += transform.position.normalized * (_noiseValue.x * _noiseAmplitude);
            }
        }
        
        // Enter animation
        // float enterAnimationPhase = (Time.time - _prevTime) / _enterAnimationDuration; 
        //
        // Debug.Log(enterAnimationPhase);
        //
        // if(enterAnimationPhase < 1f)
        // {
        //     _enterPhasePosition = Vector3.Lerp(_startPos, _newPosition, enterAnimationPhase);
        //     transform.position = _enterPhasePosition;
        // }
        // else
        // {
        //     transform.position = _newPosition;            
        // }

        
        // Adding Depth 
        if (_isDepthEnabled)
        {
            _cameraDirection = (_mainCameraTransform.position - transform.position).normalized;
            _newPosition += _cameraDirection * (_depthAmplitude * Mathf.Sin(_phase));    
        }
        
        transform.position = _newPosition;
        
        //Debug.DrawLine(_prevPosition, transform.position, Color.cyan, _debugTrailGizmoDelay);
        //Debug.DrawLine(_mainCameraTransform.position, transform.position, Color.yellow, 0.01f);
    }

}
