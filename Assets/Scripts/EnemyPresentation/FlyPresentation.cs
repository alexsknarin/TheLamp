using UnityEngine;

[RequireComponent(typeof(PreAttackFlash))]
[RequireComponent(typeof(FlyMovement))]
public class FlyPresentation : MonoBehaviour
{
    private FlyMovement _flyMovement;
    private PreAttackFlash _preAttackFlash;

    private void OnEnable()
    {
        _preAttackFlash = GetComponent<PreAttackFlash>();
        _flyMovement = GetComponent<FlyMovement>();
        
        _flyMovement.OnPreAttackStart += _preAttackFlash.PreAttackStart;
        _flyMovement.OnPreAttackEnd += _preAttackFlash.PreAttackEnd;
    }
    
    private void OnDisable()
    {
        _flyMovement.OnPreAttackStart -= _preAttackFlash.PreAttackStart;
        _flyMovement.OnPreAttackEnd -= _preAttackFlash.PreAttackEnd;
    }
}
