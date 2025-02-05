using System;
using UnityEngine;

public class FlyMovementStateFactory
{
    private Transform _cameraTransform;
    private ILampPositionProviderService _lampPositionProviderService;
    private IPositionDirectionProvider _positionDirectionProvider;
    private float _speed;
    private float _radius;
    private float _verticalAmplitude;
    private Vector2 _spawnAreaCenter;
    private float _spawnAreaSize;
    private float _proximityOffset;
    private bool _isDeathByTimer;

    public FlyMovementStateFactory(
        Transform cameraTransform, 
        ILampPositionProviderService lampPositionProviderService
        )
    {
        _cameraTransform = cameraTransform;
        _lampPositionProviderService = lampPositionProviderService;
    }
    
    public void SetEnemyDependencies(
        IPositionDirectionProvider positionDirectionProvider,
        float speed,
        float radius,
        float verticalAmplitude,
        float proximityOffset,
        bool isDeathByTimer
        )
    {
        _positionDirectionProvider = positionDirectionProvider;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _proximityOffset = proximityOffset;
        _isDeathByTimer = isDeathByTimer;
    }
    
    
    public FFlyMovementStateBase Create(Type stateType)
    {
        if (stateType == typeof(FFlyMovementEnterState))
        {
            return new FFlyMovementEnterState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _radius,
                _verticalAmplitude
                );
        }
        if (stateType == typeof(FFlyMovementPatrolState))
        {
            return new FFlyMovementPatrolState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _radius,
                _verticalAmplitude
                );
        }
        if (stateType == typeof(FFlyMovementPreAttackStateR))
        {
            return new FFlyMovementPreAttackStateR(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed
                );
        }
        if (stateType == typeof(FFlyMovementPreAttackStateL))
        {
            return new FFlyMovementPreAttackStateL(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed
            );
        }
        if (stateType == typeof(FFlyMovementAttackState))
        {
            return new FFlyMovementAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _proximityOffset
                );
        }
        if (stateType == typeof(FFlyMovementFallState))
        {
            return new FFlyMovementFallState(
                _positionDirectionProvider,
                _lampPositionProviderService,
                _radius,
                _verticalAmplitude
                );
        }
        if (stateType == typeof(FFlyMovementDeathState))
        {
            return new FFlyMovementDeathState(
                _positionDirectionProvider,
                _isDeathByTimer
                );
        }
        if (stateType == typeof(FFlyMovementSpreadState))
        {
            return new FFlyMovementSpreadState(
                _positionDirectionProvider,
                _speed
                );
        }
        return null;
    }
}
