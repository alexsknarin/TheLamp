using System;

public class SpiderMovementStateFactory
{
    private IPositionDirectionProvider _positionDirectionProvider;
    private float _speed;
    private float _xCenter;
    private float _height;
    
    public void SetEnemyDependencies(
        IPositionDirectionProvider positionDirectionProvider,
        float speed,
        float xCenter,
        float height
    )
    {
        _positionDirectionProvider = positionDirectionProvider;
        _speed = speed;
        _xCenter = xCenter;
        _height = height;
    }

    public RegularEnemyMovementStateBase Create(Type stateType)
    {
        if (stateType == typeof(FSpiderMovementEnterState))
        {
            return new FSpiderMovementEnterState(
                _speed,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(FSpiderMovementPatrolState))
        {
            return new FSpiderMovementPatrolState(
                _positionDirectionProvider,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(FSpiderMovementPreAttackState))
        {
            return new FSpiderMovementPreAttackState(
                _positionDirectionProvider,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(FSpiderMovementAttackState))
        {
            return new FSpiderMovementAttackState(
                _positionDirectionProvider,
                _speed,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(FSpiderMovementReturnState))
        {
            return new FSpiderMovementReturnState(
                _positionDirectionProvider,
                _speed,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(FFlyGenericMovementDeathState))
        {
            return new FFlyGenericMovementDeathState(
                _positionDirectionProvider,
                false
            );
        }
        if (stateType == typeof(FSpiderMovementClimbUpState))
        {
            return new FSpiderMovementClimbUpState(
                _positionDirectionProvider,
                _speed,
                _height
            );
        }
        
        return null;
    }
}
