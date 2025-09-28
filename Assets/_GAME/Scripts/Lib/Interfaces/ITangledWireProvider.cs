using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface ITangledWireProvider
    {
        public Vector3 StartPoint { get; }
        public List<Vector3> CollisionPoints { get; }
        public Vector3 EndPoint { get;  }
    }
}
