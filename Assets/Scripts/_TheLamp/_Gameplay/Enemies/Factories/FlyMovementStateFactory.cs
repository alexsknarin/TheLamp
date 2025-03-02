using System;
using UnityEngine;

public class FlyMovementStateFactory
{
    private readonly Transform _cameraTransform;
    private readonly ILampPositionProviderService _lampPositionProviderService;
    private IPositionDirectionProvider _positionDirectionProvider;
    private float _speed;
    private float _radius;
    private float _verticalAmplitude;
    private float _proximityOffset;
    private bool _isDeathByTimer;
    private float _collisionRadius;
    private float _preAttackDuration;
    private float _fallBounceForce;
    private float _fallGravityForce;

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
        bool isDeathByTimer,
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
        _proximityOffset = proximityOffset;
        _isDeathByTimer = isDeathByTimer;
        _collisionRadius = collisionRadius;
        _preAttackDuration = preAttackDuration;
        _fallBounceForce = fallBounceForce;
        _fallGravityForce = fallGravityForce;
    }
    
    
    public RegularEnemyMovementStateBase Create(Type stateType)
    {
        if (stateType == typeof(FFlyGenericMovementEnterState))
        {
            return new FFlyGenericMovementEnterState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
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
        if (stateType == typeof(FFlyGenericMovementPreAttackStateR))
        {
            return new FFlyGenericMovementPreAttackStateR(
                _cameraTransform.position,
                _positionDirectionProvider,
                _speed,
                _preAttackDuration
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
        if (stateType == typeof(FFlyGenericMovementAcceleratedAttackState))
        {
            return new FFlyGenericMovementAcceleratedAttackState(
                _cameraTransform.position,
                _positionDirectionProvider,
                _lampPositionProviderService,
                _speed,
                _proximityOffset
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
        if (stateType == typeof(FFlyGenericMovementDeathState))
        {
            return new FFlyGenericMovementDeathState(
                _positionDirectionProvider,
                _isDeathByTimer
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
