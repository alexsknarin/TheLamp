using System;
using Unity.IntegerTime;
using UnityEngine;

public abstract class FLadybugMovementPreAttackState : RegularEnemyMovementStateBase
{
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    private readonly ILampPositionProviderService _lampPositionProviderService;
    private readonly float _speed;

    private float _duration = .30f;
    private float _acceleratedSpeed;
    private float _acceleration = 0.93f;
    private Vector2 _direction;
    private Vector2 _tangentDirection;
    private float _localTime;

    public event Action Started;
    public event Action Ended;
    
    public FLadybugMovementPreAttackState(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService,
        float speed
    )
    {
        // _cameraPosition = cameraPosition; TODO: enable later
        _positionDirectionProvider = positionDirectionProvider;
        _lampPositionProviderService = lampPositionProviderService;
        _speed = speed;
    }
    
    protected void HandleEnter(int sideDirection)
    {
        IsReadyToSwitch = false;
        _localTime = 0;
        _acceleratedSpeed = 1f;
        Position2D = _positionDirectionProvider.Position2D;
        
        Vector3 direction = (Position2D - _lampPositionProviderService.GetLampPosition()).normalized; 
        Quaternion rotation = Quaternion.Euler(0, 0, 90 * sideDirection);
        _tangentDirection = rotation * direction;
        _direction = direction;
        Started?.Invoke();
    }

    public override void Tick()
    {
        Vector2 direction;
        float phase = _localTime / _duration;
        
        if (phase < 0.5f)
        {
            direction = Vector2.Lerp(_tangentDirection, _direction, phase * 2).normalized;
        }
        else
        {
            direction = Vector2.Lerp(_direction, -_tangentDirection , (phase - 0.5f) * 2).normalized;
        }
        
        Position2D += direction * (_speed * Time.deltaTime * (Mathf.PI/2) * _acceleratedSpeed);
        
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * 0.1f;
        _acceleratedSpeed *= _acceleration;
        
        if (phase > 1.1f)
        {
            IsReadyToSwitch = true;
        }
        
        _localTime += Time.deltaTime;
    }

    public override void OnExit()
    {
        Ended?.Invoke();
    }
}
