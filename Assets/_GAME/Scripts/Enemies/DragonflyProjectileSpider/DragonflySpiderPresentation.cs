using UnityEngine;

public class DragonflySpiderPresentation : MonoBehaviour, IInitializable
{
    [SerializeField] private DeathFlash _deathFlash; 
    [Header("------ Preattack Flash ------")]
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DragonflySpiderWebHandler _spiderWeb;
    
    public void Initialize()
    {
        _deathFlash.Initialize();
        _preAttackFlash.Initialize();
        _spiderWeb.Initialize();
    }
    
    public void Play()
    {
        _spiderWeb.Play(transform);
        _deathFlash.Initialize();
    }
    
    public void SwitchToCaughtState()
    {
        _spiderWeb.StartShrink();
    }
    
    public void PreAttackStart()
    {
        _preAttackFlash.PreAttackStart();
    }

    public void PreAttackEnd()
    {
        _preAttackFlash.PreAttackEnd();
    }

    public void DeathFlash()
    {
        _deathFlash.Play();
    }
}
