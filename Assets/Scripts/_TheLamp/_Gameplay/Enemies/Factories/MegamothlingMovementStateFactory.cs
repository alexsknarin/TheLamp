using System;
using UnityEngine;

public class MegamothlingMovementStateFactory
{
    private Transform _cameraTransform;
    private ILampPositionProviderService _lampPositionProviderService;
    private IPositionDirectionProvider _positionDirectionProvider;
    private float _speed;
    private float _radius;
    private float _verticalAmplitude;
    private float _collisionRadius;
    private float _preAttackDuration;
    private float _fallBounceForce;
    private float _fallGravityForce;
    
    public MegamothlingMovementStateFactory(
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
        float collisionRadius,
        float preAttackDuration,
        float fallBounceForce,
        float fallGravityForce
    )
    {
        _positionDirectionProvider = positionDirectionProvider;
        _speed = speed;
        _radius = radius;
        _verticalAmplitude = verticalAmplitude;
        _collisionRadius = collisionRadius;
        _preAttackDuration = preAttackDuration;
        _fallBounceForce = fallBounceForce;
        _fallGravityForce = fallGravityForce;
    }

    public RegularEnemyMovementStateBase Create(Type stateType)
    {
        if (stateType == typeof(FMegamothlingMovementEnterState))
        {
            return new FMegamothlingMovementEnterState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(FFlyGenericMovementPatrolState))
        {
            return new FFlyGenericMovementPatrolState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _radius,
                _verticalAmplitude
            );
        }
        if (stateType == typeof(FFlyGenericMovementPreAttackStateL))
        {
            return new FFlyGenericMovementPreAttackStateL(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _preAttackDuration
            );
        }
        if (stateType == typeof(FFlyGenericMovementPreAttackStateR))
        {
            return new FFlyGenericMovementPreAttackStateR(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _preAttackDuration
            );
        }
        if (stateType == typeof(FMegamothlingMovementAttackState))
        {
            return new FMegamothlingMovementAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed
            );
        }
        if (stateType == typeof(FFlyGenericMovementFallState))
        {
            return new FFlyGenericMovementFallState(
                _positionDirectionProvider,
                _lampPositionProviderService,
                _radius,
                _verticalAmplitude,
                _collisionRadius,
                _fallBounceForce,
                _fallGravityForce
            );
        }
        if (stateType == typeof(FMegamothlingMovementDeathState))
        {
            return new FMegamothlingMovementDeathState(
                _positionDirectionProvider
            );
        }
        return null;
    }
}
