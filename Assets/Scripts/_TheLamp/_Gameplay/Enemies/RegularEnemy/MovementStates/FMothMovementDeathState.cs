using System;
using UnityEngine;

public class FMothMovementDeathState: RegularEnemyMovementStateBase
{
    // Dependencies
    private IPositionDirectionProvider _positionDirectionProvider;
    
    // State specific attributes
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private float _bounceForceMagnitude = 4f;
    private float _gravityForceMagnitude = .2f;
    private float _dragAmount = 0.94f;
    private float _noiseFrequency = 7f;
    private float _noiseAmplitude = 0.035f;
    
    public FMothMovementDeathState(IPositionDirectionProvider positionDirectionProvider)
    {
        _positionDirectionProvider = positionDirectionProvider;
    }

    public event Action Ended;
    
    public override void OnEnter()
    {
        Position2D = _positionDirectionProvider.Position2D;
        DepthDirection = Vector3.zero;
        _bounceForce = Position2D.normalized * _bounceForceMagnitude;
        _gravityForce = Vector3.zero;
    }

    public override void Tick()
    {
        Position2D += _bounceForce * Time.deltaTime + _gravityForce;
        // Add noise
        Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
        Position2D += trajectoryNoise * _noiseAmplitude;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
        if (Position2D.y < -4f)
        {
            Ended?.Invoke();
        }
    }
}
