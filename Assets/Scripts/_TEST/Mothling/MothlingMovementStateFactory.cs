using System;
using UnityEngine;

public class MothlingMovementStateFactory
{
    private Transform _cameraTransform;
    private ILampPositionProviderService _lampPositionProviderService;
    
    private IPosition2DProvider _position2DProvider;
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
        IPosition2DProvider position2DProvider,
        float speed,
        float radius,
        float verticalAmplitude
        )
    {
        _position2DProvider = position2DProvider;
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
                _position2DProvider,
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
                _position2DProvider,
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
                _position2DProvider
                );
        }
        if (stateType == typeof(FMothlingMovementAttackState))
        {
            return new FMothlingMovementAttackState(
                _cameraTransform.position,
                _position2DProvider,
                _lampPositionProviderService,
                _speed
                );
        }
        if (stateType == typeof(FMothlingMovementFallState))
        {
            return new FMothlingMovementFallState(
                _position2DProvider,
                _radius,
                _verticalAmplitude
                );
        }
        return null;
    }
    
    
    
}
