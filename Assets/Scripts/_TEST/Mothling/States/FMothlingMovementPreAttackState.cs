using UnityEngine;

public class FMothlingMovementPreAttackState: FMothlingMovementStateBase
{
    // Dependencies
    private readonly IPosition2DProvider _positionProvider;
    
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f); // DI

    // State specific attributes
    private float _duration = .35f;
    private float _acceleratedSpeed;
    private float _acceleration = 0.93f;
    private Vector3 _direction;
    private float _localTime;

    public FMothlingMovementPreAttackState(IPosition2DProvider positionProvider)
    {
        _positionProvider = positionProvider;
    }

    public override void OnEnter()
    {
        ReadyToSwitch = false;
        _acceleratedSpeed = 1f;
        _direction = _positionProvider.Position2D.normalized;
        Quaternion rotation = Quaternion.Euler(0, 0, 60);
        _direction = rotation * _direction;
        
        Position2D = _positionProvider.Position2D;
        _localTime = 0;
    }

    public override void Tick()
    {
        Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        DepthDirection = cameraDirection * 1.0f;
        _acceleratedSpeed *= _acceleration;
        _localTime += Time.deltaTime;
        
        if (_localTime > _duration)
        {
            ReadyToSwitch = true;
        }
    }
}
