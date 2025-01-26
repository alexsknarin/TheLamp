using UnityEngine;

public class FMothlingMovementPreAttackState: FMothlingMovementStateBase
{
    // Dependencies
    private readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);
    private readonly IPosition2DProvider _position2DProvider;

    // State specific attributes
    private readonly float _duration = .35f;
    private readonly float _acceleration = 0.93f;
    private float _acceleratedSpeed;
    private Vector2 _direction;
    private float _localTime;

    public FMothlingMovementPreAttackState(
        Vector3 cameraPosition,
        IPosition2DProvider position2DProvider
        )
    {
        // _cameraPosition = cameraPosition; // TODO: enable later
        _position2DProvider = position2DProvider;
    }

    public override void OnEnter()
    {
        ReadyToSwitch = false;
        _acceleratedSpeed = 1f;
        _direction = _position2DProvider.Position2D.normalized;
        Quaternion rotation = Quaternion.Euler(0, 0, 60);
        _direction = rotation * _direction;
        
        Position2D = _position2DProvider.Position2D;
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
