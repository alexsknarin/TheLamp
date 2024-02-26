using UnityEngine;

public abstract class EnemyPresentation : MonoBehaviour
{
    public abstract void PreAttackStart();
    public abstract void PreAttackEnd();
    public abstract void DamageFlash();
    public abstract void DeathFlash();
}
