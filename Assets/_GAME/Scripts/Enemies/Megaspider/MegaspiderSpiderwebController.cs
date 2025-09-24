using System;
using _GAME.Scripts.Enemies.Megaspider.Data;
using _GAME.Scripts.Enemies.Megaspider.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderSpiderwebController : MonoBehaviour, IInitializable
    {
        [SerializeField] private MegaspiderMovement _movement;
        [SerializeField] private MegaspiderSpiderweb _spiderweb1;
        [SerializeField] private MegaspiderSpiderweb _spiderweb2;
        [SerializeField] private MegaspiderSpiderweb _spiderweb3;
        [SerializeField] private MegaspiderSpiderweb _spiderweb4;
        [SerializeField] private MegaspiderSpiderweb _spiderweb5;
        [SerializeField] private MegaspiderSpiderweb _spiderweb6;
        [SerializeField] private MegaspiderSpiderwebPointsConfig _config;


        public void Initialize()
        {
            _movement.StaticBridge1Called += OnStaticBridge1Called;
            _movement.StaticBridge1Broken += OnStaticBridge1Broken;
            _movement.StaticBridge2Called += OnStaticBridge2Called;
            _movement.StaticBridge2Broken += OnStaticBridge2Broken;
            _movement.StaticBridge3Called += OnStaticBridge3Called;
            _movement.StaticBridge3Broken += OnStaticBridge3Broken;
            _movement.HangStartRequested += OnHangStartRequested;
            _movement.HangStopRequested += OnHangStopRequested;
            _movement.HangBreakRequested += OnHangBreakHangRequested;
        }

        private void OnDestroy()
        {
            _movement.StaticBridge1Called -= OnStaticBridge1Called;
            _movement.StaticBridge1Broken -= OnStaticBridge1Broken;
            _movement.StaticBridge2Called -= OnStaticBridge2Called;
            _movement.StaticBridge2Broken -= OnStaticBridge2Broken;
            _movement.StaticBridge3Called -= OnStaticBridge3Called;
            _movement.StaticBridge3Broken -= OnStaticBridge3Broken;
            _movement.HangStartRequested -= OnHangStartRequested;
            _movement.HangStopRequested -= OnHangStopRequested;
            _movement.HangBreakRequested -= OnHangBreakHangRequested;
        }

        private void OnStaticBridge1Called(Type state)
        {
            if (state == typeof(MegaspiderEnterLState))
            {
                _spiderweb1.StartShootStatic(_config.Enter01LStartPoint, _config.Enter01LEndPoint);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb1.StartShootStatic(_config.Enter01RStartPoint, _config.Enter01REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileBottomAttackLState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileBottomAttackLStartPoint, _config.ProjectileBottomAttackLEndPoint);          
            }
            else if (state == typeof(MegaspiderProjectileBottomAttackRState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileBottomAttackRStartPoint, _config.ProjectileBottomAttackREndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackLState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileDoubleDown01LStartPoint, _config.ProjectileDoubleDown01LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackRState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileDoubleDown01RStartPoint, _config.ProjectileDoubleDown01REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackLState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileDoubleUp01LStartPoint, _config.ProjectileDoubleUp01LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackRState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileDoubleUp01RStartPoint, _config.ProjectileDoubleUp01REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileTopAttackLState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileTopAttackLStartPoint, _config.ProjectileTopAttackLEndPoint);          
            }
            else if (state == typeof(MegaspiderProjectileTopAttackRState))
            {
                _spiderweb1.StartShootStatic(_config.ProjectileTopAttackRStartPoint, _config.ProjectileTopAttackREndPoint);          
            }
            else if (state == typeof(MegaspiderZigzagAttackLState))
            {
                _spiderweb1.StartShootStatic(_config.ZigzagAttack01LStartPoint, _config.ZigzagAttack01LEndPoint);          
            }
            else if (state == typeof(MegaspiderZigzagAttackRState))
            {
                _spiderweb1.StartShootStatic(_config.ZigzagAttack01RStartPoint, _config.ZigzagAttack01REndPoint);          
            }
        }

        private void OnStaticBridge1Broken()
        {
            _spiderweb1.StartBreakStatic();
        }

        private void OnStaticBridge2Called(Type state)
        {
            if (state == typeof(MegaspiderEnterLState))
            {
                _spiderweb2.StartShootStatic(_config.Enter02LStartPoint, _config.Enter02LEndPoint);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb2.StartShootStatic(_config.Enter02RStartPoint, _config.Enter02REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackLState))
            {
                _spiderweb2.StartShootStatic(_config.ProjectileDoubleDown02LStartPoint, _config.ProjectileDoubleDown02LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackRState))
            {
                _spiderweb2.StartShootStatic(_config.ProjectileDoubleDown02RStartPoint, _config.ProjectileDoubleDown02REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackLState))
            {
                _spiderweb2.StartShootStatic(_config.ProjectileDoubleUp02LStartPoint, _config.ProjectileDoubleUp02LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackRState))
            {
                _spiderweb2.StartShootStatic(_config.ProjectileDoubleUp02RStartPoint, _config.ProjectileDoubleUp02REndPoint);
            }
            else if (state == typeof(MegaspiderZigzagAttackLState))
            {
                _spiderweb2.StartShootStatic(_config.ZigzagAttack02LStartPoint, _config.ZigzagAttack02LEndPoint);          
            }
            else if (state == typeof(MegaspiderZigzagAttackRState))
            {
                _spiderweb2.StartShootStatic(_config.ZigzagAttack02RStartPoint, _config.ZigzagAttack02REndPoint);          
            }
        }

        private void OnStaticBridge2Broken()
        {
            _spiderweb2.StartBreakStatic();
        }

        private void OnStaticBridge3Called(Type state)
        {
            if (state == typeof(MegaspiderZigzagAttackLState))
            {
                _spiderweb3.StartShootStatic(_config.ZigzagAttack03LStartPoint, _config.ZigzagAttack03LEndPoint);          
            }
            else if (state == typeof(MegaspiderZigzagAttackRState))
            {
                _spiderweb3.StartShootStatic(_config.ZigzagAttack03RStartPoint, _config.ZigzagAttack03REndPoint);          
            }
        }
        
        private void OnStaticBridge3Broken()
        {
            _spiderweb3.StartBreakStatic();
        }

        private void OnHangStartRequested(Type state, IPositionProvider endPositionProvider)
        {
            if (state == typeof(MegaspiderEnterLState))
            {
                _spiderweb6.StartHang(_config.EnterHangLStartPoint, endPositionProvider);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb6.StartHang(_config.EnterHangRStartPoint, endPositionProvider);          
            }
            if (state == typeof(MegaspiderHangAttackLState))
            {
                _spiderweb6.StartHang(_config.HangAttackLStartPoint, endPositionProvider);          
            }
            if (state == typeof(MegaspiderHangAttackRState))
            {
                _spiderweb6.StartHang(_config.HangAttackRStartPoint, endPositionProvider);          
            }
        }

        private void OnHangStopRequested()
        {
            _spiderweb6.StopHang();
        }

        private void OnHangBreakHangRequested()
        {
            _spiderweb6.StartBreakHang();
        }
    }
}
