using _GAME.Scripts.Enemies.Generic.Presentation;
using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileMoth
{
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
}
