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
        if (stateType == typeof(MothMovementEnterState))
        {
            return new MothMovementEnterState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(MothMovementHoverState))
        {
            return new MothMovementHoverState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _radius
            );
        }
        if (stateType == typeof(MothMovementNoisePatrolState))
        {
            return new MothMovementNoisePatrolState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(MothMovementPreAttackState))
        {
            return new MothMovementPreAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed
            );
        }
        if (stateType == typeof(MothMovementNoiseAttackState))
        {
            return new MothMovementNoiseAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed
            );
        }
        if (stateType == typeof(MothMovementNoiseFallState))
        {
            return new MothMovementNoiseFallState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(MothMovementNoiseDeathState))
        {
            return new MothMovementNoiseDeathState(
                _positionDirectionProvider
            );
        }
        if (stateType == typeof(MothMovementNoiseSpreadState))
        {
            return new MothMovementNoiseSpreadState(
                _positionDirectionProvider,
                _speed
            );
        }
        return null;
    }


}
