using UnityEngine;

public class FMothlingMovementFallState: FMothlingMovementStateBase
{
    private IPosition2DProvider _positionProvider;
    public FMothlingMovementFallState(IPosition2DProvider positionProvider)
    {
        _positionProvider = positionProvider;
    }
    
    public Vector2 _spawnAreaCenter = new Vector2(3.6f, -2.6f);
    private float _speed = 0.81f;
    private float _radius = 1.55f;
    private float _verticalAmplitude = 0.93f;
    public float _spawnAreaSize = 0.5f;
    
    // State specific attributes
    private Vector2 _bounceForce;
    private Vector2 _gravityForce;
    private float _bounceForceMagnitude = 4f;
    private float _gravityForceMagnitude = .2f;
    private float _dragAmount = 0.94f;
    
    public override void OnEnter()
    {
        _bounceForce = _positionProvider.Position2D.normalized * _bounceForceMagnitude;
        _gravityForce = Vector3.zero;
        ReadyToSwitch = false;
    }

    public override void Tick()
    {
        Position2D = Position2D + _bounceForce * Time.deltaTime + _gravityForce;
        _bounceForce *= _dragAmount;
        _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
   
        if (Position2D.y < -_radius * _verticalAmplitude * 1.1)
        {
            ReadyToSwitch = true;
        }
    }
}
