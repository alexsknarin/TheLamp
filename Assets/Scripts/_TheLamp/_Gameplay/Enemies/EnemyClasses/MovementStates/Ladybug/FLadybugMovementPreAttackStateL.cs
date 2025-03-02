using UnityEngine;

public class FLadybugMovementPreAttackStateL : FLadybugMovementPreAttackState
{
    public FLadybugMovementPreAttackStateL(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider, 
        ILampPositionProviderService lampPositionProviderService,
        float speed) : 
        base(
            cameraPosition,
            positionDirectionProvider, 
            lampPositionProviderService,
            speed)
    {
    }

    public override void OnEnter()
    {
        HandleEnter(-1);
    }
}
