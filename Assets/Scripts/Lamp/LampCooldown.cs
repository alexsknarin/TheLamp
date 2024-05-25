using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampCooldown : MonoBehaviour
{
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _lampCooldownAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    
    public void PerformCooldown(float phase)
    {
        _lampEmissionController.Intensity = _lampCooldownAnimCurve.Evaluate(phase);
        _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
    }
}
