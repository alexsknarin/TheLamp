using UnityEngine;

[RequireComponent(typeof(PreAttackFlash))]
[RequireComponent(typeof(MothMovement))]
public class MothPresentation : MonoBehaviour
{
    private IPreAttackStateProvider _mothMovement;
    private PreAttackFlash _preAttackFlash;

    private void OnEnable()
    {
        _preAttackFlash = GetComponent<PreAttackFlash>();
        _mothMovement = GetComponent<MothMovement>();
        
        _mothMovement.OnPreAttackStart += _preAttackFlash.PreAttackStart;
        _mothMovement.OnPreAttackEnd += _preAttackFlash.PreAttackEnd;
    }
    
    private void OnDisable()
    {
        _mothMovement.OnPreAttackStart -= _preAttackFlash.PreAttackStart;
        _mothMovement.OnPreAttackEnd -= _preAttackFlash.PreAttackEnd;
    }
}