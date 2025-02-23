using UnityEngine;

public interface IStickableWithLamp
{
    public Vector2 Position { get; }
    public float Radius { get; }
    public bool IsSticked { get; }
    public AttackBlockerState AttackBlockState { get; }
    public StickableState StickState { get; }
    public void HandleEnterAttackZone();
    public void HandleStick(Transform lampTransform);
    public void HandleExitAttackZone();
    public void HandleEnterAttackBlockerZone();
    public void HandleLampDestroyed();
    public Vector3 ProvideImpactPoint();
}
