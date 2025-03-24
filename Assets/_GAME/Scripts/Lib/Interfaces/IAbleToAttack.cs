namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IAbleToAttack
    {
        public bool IsReadyToAttack { get; }
        public void Attack();
    }
}
