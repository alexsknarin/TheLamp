using System;
using UnityEngine;

public class LadybugMovementStateFactory
{
    private Transform _cameraTransform;
    private IPositionDirectionProvider _positionDirectionProvider;
    private ILampPositionProviderService _lampPositionProviderService;
    private float _speed;
    private float _radius;
    private float _verticalAmplitude;
    private float _collisionRadius;
    
    public LadybugMovementStateFactory(
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
        float collisionRadius
        )
    {
        _positionDirectionProvider = positionDirectionProvider;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _collisionRadius = collisionRadius;
    }

    public RegularEnemyMovementStateBase Create(Type stateType)
    {
        if (stateType == typeof(FLadybugMovementPatrolStateR))
        {
            return new FLadybugMovementPatrolStateR(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(FLadybugMovementPatrolStateL))
        {
            return new FLadybugMovementPatrolStateL(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(FLadybugMovementPreAttackStateR))
        {
            return new FLadybugMovementPreAttackStateR(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed
            );
        }
        if (stateType == typeof(FLadybugMovementPreAttackStateL))
        {
            return new FLadybugMovementPreAttackStateL(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed
            );
        }
        if (stateType == typeof(FLadybugMovementAttackState))
        {
            return new FLadybugMovementAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed
            );
        }
        if (stateType == typeof(FLadybugMovementStickState))
        {
            return new FLadybugMovementStickState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _collisionRadius
            );
        }
        if (stateType == typeof(FLadybugMovementDeathState))
        {
            return new FLadybugMovementDeathState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService
            );
        }
        if (stateType == typeof(FFlyGenericMovementSpreadState))
        {
            return new FFlyGenericMovementSpreadState(
                _positionDirectionProvider,
                _speed
            );
        }
        return null;
    }
    
}
