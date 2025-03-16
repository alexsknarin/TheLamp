using System;
using UnityEngine;

public class LadybugMovementDeathFallState: RegularEnemyMovementStateBase
{
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    IPositionDirectionProvider _positionDirectionProvider;
    private readonly ILampPositionProviderService _lampPositionProviderService;


    private readonly float _depth = 0.3f; // TODO: move to config

    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private float _bounceForceMagnitude = 3f;
    private float _gravityForceMagnitude = .17f;
    private float _dragAmount = 0.9f;
    
    public LadybugMovementDeathFallState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService
    )
    {
        // _cameraPosition = cameraPosition; TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
    }
    
    public event Action Ended;

    public override void OnEnter()
    {
        Position2D = _positionDirectionProvider.Position2D;
        _bounceForce = (Position2D - _lampPositionProviderService.GetLampPosition()).normalized * _bounceForceMagnitude;
        _gravityForce = Vector2.zero;
        DepthDirection = _positionDirectionProvider.DepthDirection;
    }

    public override void Tick()
    {
        Position2D += _bounceForce * Time.deltaTime + _gravityForce;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * _depth;
        
        if (Position2D.y < -4f)
        {
            Ended?.Invoke();
        }
    }
}
