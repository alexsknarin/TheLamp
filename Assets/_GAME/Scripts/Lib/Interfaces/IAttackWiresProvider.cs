using System.Collections.Generic;
using _GAME.Scripts.Enemies.Megaspider;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IAttackWiresProvider
    {
        public List<SpiderwebAttackWire> AttackWires { get;  }
    }
}
