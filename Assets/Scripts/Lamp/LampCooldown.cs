using UnityEngine;

public class LampCooldown : MonoBehaviour
{
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _lampCooldownAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    
    public void PerformCooldown(float phase, bool isBlockedAttack)
    {
        _lampEmissionController.Intensity = _lampCooldownAnimCurve.Evaluate(phase);
        if (isBlockedAttack)
        {
            return;    
        }
        _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
    }
}
