using UnityEngine;

public class FlyPresentation : EnemyPresentation
{
    [SerializeField] private PreAttackFlash _preAttackFlash;
   
    public override void PreAttackStart()
    {
        _preAttackFlash.PreAttackStart();
    }
    
    public override void PreAttackEnd()
    {
        _preAttackFlash.PreAttackEnd();
    }
}
