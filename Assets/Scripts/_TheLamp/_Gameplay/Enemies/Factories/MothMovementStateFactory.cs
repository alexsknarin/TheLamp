using System;
using UnityEngine;

public class MothMovementStateFactory
{
    private Transform _cameraTransform;
    private ILampPositionProviderService _lampPositionProviderService;
    private IPositionDirectionProvider _positionDirectionProvider;
    private float _speed;
    private float _radius;
    private float _verticalAmplitude;
    
    public MothMovementStateFactory(
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

    public RegularEnemyMovementStateBase Create(Type stateType)
    {
        if (stateType == typeof(FMothMovementEnterState))
        {
            return new FMothMovementEnterState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(FMothMovementHoverState))
        {
            return new FMothMovementHoverState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _radius
            );
        }
        if (stateType == typeof(FMothMovementPatrolState))
        {
            return new FMothMovementPatrolState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(FMothMovementPreAttackState))
        {
            return new FMothMovementPreAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed
            );
        }
        if (stateType == typeof(FMothMovementAttackState))
        {
            return new FMothMovementAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed
            );
        }
        if (stateType == typeof(FMothMovementFallState))
        {
            return new FMothMovementFallState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(FMothMovementDeathState))
        {
            return new FMothMovementDeathState(
                _positionDirectionProvider
            );
        }
        return null;
    }


}
