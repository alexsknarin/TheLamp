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
        if (stateType == typeof(SpiderMovementEnterState))
        {
            return new SpiderMovementEnterState(
                _speed,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(SpiderMovementPatrolState))
        {
            return new SpiderMovementPatrolState(
                _positionDirectionProvider,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(SpiderMovementPreAttackState))
        {
            return new SpiderMovementPreAttackState(
                _positionDirectionProvider,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(SpiderMovementAttackState))
        {
            return new SpiderMovementAttackState(
                _positionDirectionProvider,
                _speed,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(SpiderMovementReturnState))
        {
            return new SpiderMovementReturnState(
                _positionDirectionProvider,
                _speed,
                _xCenter,
                _height
            );
        }
        if (stateType == typeof(FlyGenericMovementDeathState))
        {
            return new FlyGenericMovementDeathState(
                _positionDirectionProvider,
                false
            );
        }
        if (stateType == typeof(SpiderMovementClimbUpState))
        {
            return new SpiderMovementClimbUpState(
                _positionDirectionProvider,
                _speed,
                _height
            );
        }
        
        return null;
    }
}
