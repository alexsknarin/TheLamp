using UnityEngine;

public class FMegabeetleMovementEnterStateR : FMegabeetleMovementEnterState
{
    public FMegabeetleMovementEnterStateR(
        Vector3 cameraPosition,
        IPositionDirectionProvider positionDirectionProvider,
        ILampPositionProviderService lampPositionProviderService, 
        float speed,
        float radius,
        float verticalAmplitude) : 
        base(
            cameraPosition,
            positionDirectionProvider,
            lampPositionProviderService,
            speed,
            radius,
            verticalAmplitude
            )
    {
    }

    public override void OnEnter()
    {
        HandleEnter(1);
    }

    public override void Tick()
    {
        HandleTick(1);
    }
}
