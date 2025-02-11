using System;
using UnityEngine;

public class FMothlingMovementPreAttackState: RegularEnemyMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;

    // State specific attributes
    private readonly float _duration = .35f;
    private readonly float _acceleration = 0.93f; //TODO: remove
    private float _acceleratedSpeed; //TODO: remove
    private Vector2 _direction;
    private float _localTime;

    public FMothlingMovementPreAttackState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider
        )
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
    }

    public event Action Started;
    public event Action Ended;

    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        _acceleratedSpeed = 1f;
        _direction = _positionDirectionProvider.Position2D.normalized; // TODO: remove
        Quaternion rotation = Quaternion.Euler(0, 0, 60); // TODO: remove
        _direction = rotation * _direction; // TODO: remove
        
        Position2D = _positionDirectionProvider.Position2D;
        DepthDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        
        _localTime = 0;
        Started?.Invoke();
    }

    public override void Tick()
    {
        DepthDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        _acceleratedSpeed *= _acceleration; // TODO: remove
        _localTime += Time.deltaTime;
        
        if (_localTime > _duration)
        {
            IsReadyToSwitch = true;
        }
    }
    
    public override void OnExit()
    {
        Ended?.Invoke();
    }
}
