using System;
using UnityEngine;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IStickyAttacker
    {
        public event Action<Vector3, bool, string> StickyAttackEnded;
    }
}
