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

        return null;
    }


}
