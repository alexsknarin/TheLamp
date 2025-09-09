using UnityEngine;

namespace _GAME.Scripts.Lib
{
    public abstract class RandomDirection
    {
        public static int Generate()
        {
            return Random.Range(0, 2) * 2 - 1;
        }
    }
}
