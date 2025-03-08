using UnityEngine;

public class FMegabeetleMovementStickPreAttackPauseState : RegularEnemyMovementStateBase
{
    private readonly IPositionDirectionProvider _positionDirectionProvider;
    
    private float _duration = 0.12f;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public FMegabeetleMovementStickPreAttackPauseState(IPositionDirectionProvider positionDirectionProvider)
    {
        _positionDirectionProvider = positionDirectionProvider;
    }
    
    public override void OnEnter()
    {
        IsReadyToSwitch = false;
        Position2D = _positionDirectionProvider.Position2D;
        _localTime = 0;
        _phase = 0;
    }

    public override void Tick()
    {
        _phase  = _localTime / _duration;
        _localTime += Time.deltaTime;
        if (_phase > 1)
        {
            IsReadyToSwitch = true;
        }
        
    }
}
