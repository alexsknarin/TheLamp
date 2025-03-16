using System;
using UnityEngine;

public class MothMovementNoiseFallState: RegularEnemyMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private ILampPositionProviderService _lampPositionProviderService;
    private readonly float _radius;
    private readonly float _verticalAmplitude;
    
    
    // State specific attributes
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private float _bounceForceMagnitude = 4f;
    private float _gravityForceMagnitude = .2f;
    private float _dragAmount = 0.94f;
    private float _noiseFrequency = 7f;
    private float _noiseAmplitude = 0.015f;
    private float _localTime;
    private readonly float _yPositionToSwitch;
    
    public MothMovementNoiseFallState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float radius,
        float verticalAmplitude
    )
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        
        _yPositionToSwitch = -_radius * _verticalAmplitude * 1.1f;
    }

    public event Action Ended;
    
    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        Position2D = _positionDirectionProvider.Position2D;
        
        Vector2 position2DNormalized = (Position2D - _lampPositionProviderService.GetLampPosition()).normalized;
        Position2D = position2DNormalized * (0.49f + 0.1f + 0.0001f) + _lampPositionProviderService.GetLampPosition(); // TODO: Magic numbers
        
        _bounceForce = position2DNormalized * _bounceForceMagnitude;
        
        Debug.DrawRay(_lampPositionProviderService.GetLampPosition(), position2DNormalized, Color.red, 5f);
        
        _gravityForce = Vector3.zero;
        _localTime = 0;
    }

    public override void Tick()
    {
        Position2D += _bounceForce * Time.deltaTime + _gravityForce;
        
        // Add noise
        float noisePhase = _localTime / 1.5f; // TODO: Magic Number
        Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency) * noisePhase;
        Position2D += trajectoryNoise * _noiseAmplitude;
        
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
        _localTime += Time.deltaTime;
        
        if (Position2D.y < _yPositionToSwitch)
        {
            IsReadyToSwitch = true;
        }
    }
    
    public override void OnExit()
    {
        Ended?.Invoke();
    }
}
