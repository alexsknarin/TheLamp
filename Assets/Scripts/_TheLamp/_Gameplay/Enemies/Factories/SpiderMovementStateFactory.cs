using System;

public class SpiderMovementStateFactory
{
    private ILampPositionProviderService _lampPositionProviderService;
    private IPositionDirectionProvider _positionDirectionProvider;
    private float _speed;
    private float _xCenter;
    private float _height;
    
    public SpiderMovementStateFactory(
        ILampPositionProviderService lampPositionProviderService
        )
    {
        _lampPositionProviderService = lampPositionProviderService;
    }
    
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
        return null;
    }
}
