using UnityEngine;

public class MothMovementFallState: EnemyMovementBaseState
{ 
    public override EnemyStates State => EnemyStates.Fall;
    private Vector3 _bounceForce;
    private Vector3 _gravityForce;
    private float _bounceForceMagnitude = 4f;
    private float _gravityForceMagnitude = .2f;
    private float _dragAmount = 0.94f;
    private float _noiseFrequency = 7f;
    private float _noiseAmplitude = 0.015f;
    private float _localTime;

    public MothMovementFallState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _bounceForce = currentPosition.normalized * _bounceForceMagnitude;
        _gravityForce = Vector3.zero;
        _localTime = 0;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Position = currentPosition + _bounceForce * Time.deltaTime + _gravityForce;
        
        // Add noise
        float noisePhase = _localTime / 1.5f;
        Vector3 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency) * noisePhase;
        Position += trajectoryNoise * _noiseAmplitude;
        
        _bounceForce = _bounceForce * _dragAmount;
        _gravityForce = _gravityForce + Vector3.down * (_gravityForceMagnitude * Time.deltaTime);
        
        _localTime += Time.deltaTime;
    }
   
    public override void CheckForStateChange()
    {
        if (Position.y < -_radius * _verticalAmplitude * 1.1)
        {
            _owner.SwitchState();
        }
    }
  
}