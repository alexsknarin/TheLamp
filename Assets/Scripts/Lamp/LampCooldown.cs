using UnityEngine;

public class LampCooldown : MonoBehaviour
{
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _lampCooldownAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    [SerializeField] private MeshRenderer _lampAttackZoneRenderer;
    private Material _lampAttackZoneMaterial;
    
    private void Awake()
    {
        _lampAttackZoneMaterial = _lampAttackZoneRenderer.material;
    }
    
    public void PerformCooldown(float phase, bool isBlocked)
    {
        _lampEmissionController.Intensity = _lampCooldownAnimCurve.Evaluate(phase);
        if (isBlocked)
        {
            return;    
        }
        _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
        _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(0, 0.005f, _lampCooldownAnimCurve.Evaluate(phase)));
    }
}
