using System;
using UnityEngine;

public class MothlingMovementStateFactory
{
    private Transform _cameraTransform;
    private ILampPositionProviderService _lampPositionProviderService;
    private IPositionDirectionProvider _positionDirectionProvider;
    private float _speed;
    private float _radius;
    private float _verticalAmplitude;
    private Vector2 _spawnAreaCenter;
    private float _spawnAreaSize;

    public MothlingMovementStateFactory(
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
        float verticalAmplitude
        )
    {
        _positionDirectionProvider = positionDirectionProvider;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
    }
    
    
    public FMothlingMovementStateBase Create(Type stateType)
    {
        if (stateType == typeof(FMothlingMovementEnterState))
        {
            return new FMothlingMovementEnterState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _radius,
                _verticalAmplitude
                );
        }
        if (stateType == typeof(FMothlingMovementPatrolState))
        {
            return new FMothlingMovementPatrolState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _radius,
                _verticalAmplitude
                );
        }
        if (stateType == typeof(FMothlingMovementPreAttackState))
        {
            return new FMothlingMovementPreAttackState(
                _cameraTransform.position,
                _positionDirectionProvider
                );
        }
        if (stateType == typeof(FMothlingMovementAttackState))
        {
            return new FMothlingMovementAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed
                );
        }
        if (stateType == typeof(FMothlingMovementFallState))
        {
            return new FMothlingMovementFallState(
                _positionDirectionProvider,
                _lampPositionProviderService,
                _radius,
                _verticalAmplitude
                );
        }
        if (stateType == typeof(FMothlingMovementDeathState))
        {
            return new FMothlingMovementDeathState(
                _positionDirectionProvider
                );
        }
        if (stateType == typeof(FMothlingMovementSpreadState))
        {
            return new FMothlingMovementSpreadState(
                _positionDirectionProvider,
                _speed
                );
        }
        return null;
    }
    
    
    
}
