using System;
using UnityEngine;

public class MothMovementAttackState: EnemyMovementBaseState
{
    public override EnemyStates State => EnemyStates.Attack;
    private float _acceleration = 0.02f;
    private float _depthDecrement = 0.2f;
    private float _acceleratedSpeed = 1f;
    private float _noiseFrequency = 13f;
    private float _noiseAmplitude = 0.08f;
    private float _maxDistance = 0.5f;
    
    public MothMovementAttackState(IStateMachineOwner owner, float speed, float radius, float verticalAmplitude) : base()
    {
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _owner = owner;
    }
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _sideDirection = sideDirection;
        _acceleratedSpeed = 1f;
        _maxDistance = currentPosition.magnitude;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 newPosition = currentPosition;
        Vector3 direction = -newPosition.normalized;
        newPosition += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
        _acceleratedSpeed += _acceleration;

        // Add noise
        Vector3 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
        float noiseAttenuation = Mathf.Clamp((currentPosition.magnitude - 0.65f) / (_maxDistance - 0.65f) * 1.5f - 0.5f , 0, 1); 
        Position = newPosition + trajectoryNoise * (_noiseAmplitude * noiseAttenuation);
        Vector3 cameraDirection = (_cameraPosition - Position).normalized;
        Depth = cameraDirection * (0.2f * _depthDecrement);
    }
}