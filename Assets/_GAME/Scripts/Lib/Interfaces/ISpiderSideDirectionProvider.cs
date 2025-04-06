using _GAME.Scripts.Enemies;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface ISpiderSideDirectionProvider
    {
        public int RequestPoint(EnemyMovementBase occupant);
        public void ReleasePoint(EnemyMovementBase occupant);
    }
}
