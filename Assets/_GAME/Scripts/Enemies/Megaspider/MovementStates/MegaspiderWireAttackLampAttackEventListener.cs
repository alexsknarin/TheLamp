using System;
using _GAME.Scripts.GameCoreSystems;
using _GAME.Scripts.Lib;
using UnityEngine;
using IDisposable = _GAME.Scripts.Lib.Interfaces.IDisposable;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderWireAttackLampAttackEventListener : IDisposable
    {
        private GameModel _gameModel;

        public event Action<int> AttackStarted;
        public void SubscribeToLampAttack(GameModel gameModel)
        {
            _gameModel = gameModel;
            _gameModel.LampAttackStarted += OnAttackClicked;
        }
        
        public void Dispose()
        {
            _gameModel.LampAttackStarted -= OnAttackClicked;
        }

        private void OnAttackClicked(float power)
        {
            AttackStarted?.Invoke(Converters.PowerToAttackPower(power));
        }
    }
}
