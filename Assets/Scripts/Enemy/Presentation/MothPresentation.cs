using UnityEngine;

[RequireComponent(typeof(PreAttackFlash))]
[RequireComponent(typeof(MothMovement))]
public class MothPresentation : MonoBehaviour
{
    private IPreAttackStateProvider _preAttackStateProvider;
    private PreAttackFlash _preAttackFlash;

    private void OnEnable()
    {
        _preAttackFlash = GetComponent<PreAttackFlash>();
        _preAttackStateProvider = GetComponent<MothMovement>();
        
        _preAttackStateProvider.OnPreAttackStart += _preAttackFlash.PreAttackStart;
        _preAttackStateProvider.OnPreAttackEnd += _preAttackFlash.PreAttackEnd;
    }
    
    private void OnDisable()
    {
        _preAttackStateProvider.OnPreAttackStart -= _preAttackFlash.PreAttackStart;
        _preAttackStateProvider.OnPreAttackEnd -= _preAttackFlash.PreAttackEnd;
    }
}