using UnityEngine;

public class DragonflyMothPresentation : MonoBehaviour
{
    [SerializeField] private DeathFlash _deathFlash; 
    
    public void Initialize()
    {
        _deathFlash.Initialize();
    }
    
    public void DeathFlash()
    {
        _deathFlash.Play();
    }
}
