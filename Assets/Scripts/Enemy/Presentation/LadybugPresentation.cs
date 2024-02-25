using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeathIndication))]
public class LadybugPresentation : MonoBehaviour
{
    private IDeathStateProvider _deathStateProvider;
    private IPreAttackStateProvider _preAttackStateProvider;
    private DeathIndication _deathIndication;
    private PreAttackFlash _preAttackFlash;

    private void OnEnable()
    {
        _deathIndication = GetComponent<DeathIndication>();
        _deathStateProvider = GetComponent<LadybugMovement>();
        _preAttackStateProvider = GetComponent<LadybugMovement>();
        _preAttackFlash = GetComponent<PreAttackFlash>();
        
        _deathStateProvider.OnDeath += _deathIndication.Perform;
        _preAttackStateProvider.OnPreAttackStart += _preAttackFlash.PreAttackStart;
        _preAttackStateProvider.OnPreAttackEnd += _preAttackFlash.PreAttackEnd;
    }
    
    private void OnDisable()
    {
        _deathStateProvider.OnDeath -= _deathIndication.Perform;
        _preAttackStateProvider.OnPreAttackStart -= _preAttackFlash.PreAttackStart;
        _preAttackStateProvider.OnPreAttackEnd -= _preAttackFlash.PreAttackEnd;
    }
}
