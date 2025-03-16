using UnityEngine;

public class LadybugMovementPreAttackStateR : FLadybugMovementPreAttackState
{
    public LadybugMovementPreAttackStateR(
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
        HandleEnter(1);
    }
}
