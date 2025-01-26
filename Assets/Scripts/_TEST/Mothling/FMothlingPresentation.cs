using System;
using UnityEngine;

public class FMothlingPresentation : MonoBehaviour, IInitializable
{
    [SerializeField] private FMothlingMovement _movement;
    [SerializeField] private PreAttackFlash _preAttackFlash;
    
    public void Initialize()
    {
        _preAttackFlash.Initialize();
        
        _movement.PreAttackStarted += OnPreAttackStarted;
        _movement.PreAttackEnded += OnPreAttackEnded;
    }

    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
    }

    private void OnPreAttackStarted()
    {
        _preAttackFlash.PreAttackStart();
    }
    
    private void OnPreAttackEnded()
    {
        _preAttackFlash.PreAttackEnd();
    }
}
