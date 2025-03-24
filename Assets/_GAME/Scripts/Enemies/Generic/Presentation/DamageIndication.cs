using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public abstract class DamageIndication : MonoBehaviour, IInitializable
    {
        public abstract void Initialize();
        public abstract void Play();
    }
}
