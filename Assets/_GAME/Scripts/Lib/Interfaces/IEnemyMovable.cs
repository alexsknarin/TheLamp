namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IEnemyMovable
    {
        public void Play(); 
        public void TriggerAttack(); 
        public void TriggerFall(); 
        public void TriggerDeath();
    }
}
