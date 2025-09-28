using UnityEngine;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IStartEndPositionsProvider
    {
        public Vector3 StartPosition { get;  }
        public Vector3 EndPosition { get;  }
    }
        
}
