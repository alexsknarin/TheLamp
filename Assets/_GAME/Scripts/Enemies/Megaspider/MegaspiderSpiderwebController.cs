using System;
using _GAME.Scripts.Enemies.Megaspider.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderSpiderwebController : MonoBehaviour, IInitializable
    {
        private readonly Vector3 _enter01LStartPoint = new(-4.74f, 2.61f, 7.15f);
        private readonly Vector3 _enter01LEndPoint = new(2.71f, 1.76f, 1.76f);
        private readonly Vector3 _enter01RStartPoint = new(4.74f, 2.61f, 7.15f);
        private readonly Vector3 _enter01REndPoint = new(-2.71f, 1.76f, 1.76f);
        
        private readonly Vector3 _enter02LStartPoint = new(2.71f, 1.55f, 0.69f);
        private readonly Vector3 _enter02LEndPoint = new(-1.9f, 1.76f, -1.2f);
        private readonly Vector3 _enter02RStartPoint = new(-2.71f, 1.55f, 0.69f);
        private readonly Vector3 _enter02REndPoint = new(1.9f, 1.76f, -1.2f);
        
        private readonly Vector3 _enterHangLStartPoint = new(-0.35f, 1.05f, -3.99f);
        private readonly Vector3 _enterHangRStartPoint = new(0.35f, 1.05f, -3.99f);
        
        private readonly Vector3 _projectileBottomAttackLStartPoint = new(-1.93f, -2.34f, -1.34f);
        private readonly Vector3 _projectileBottomAttackLEndPoint = new(2.46f, -1.52f, 0.54f);
        private readonly Vector3 _projectileBottomAttackRStartPoint = new(1.93f, -2.34f, -1.34f);
        private readonly Vector3 _projectileBottomAttackREndPoint = new(-2.46f, -1.52f, 0.54f);
        
        private readonly Vector3 _projectileDoubleDown01LStartPoint = new(-3.28f, 2.6f, 1.02f);
        private readonly Vector3 _projectileDoubleDown01LEndPoint = new(2.67f, 1.38f, 0.71f);
        private readonly Vector3 _projectileDoubleDown01RStartPoint = new(3.28f, 2.6f, 1.02f);
        private readonly Vector3 _projectileDoubleDown01REndPoint = new(-2.67f, 1.38f, 0.71f);
        
        private readonly Vector3 _projectileDoubleDown02LStartPoint = new(2.46f, -1.69f, 0.28f);
        private readonly Vector3 _projectileDoubleDown02LEndPoint = new(-1.93f, -2.87f, -1.68f);
        private readonly Vector3 _projectileDoubleDown02RStartPoint = new(-2.46f, -1.69f, 0.28f);
        private readonly Vector3 _projectileDoubleDown02REndPoint = new(1.93f, -2.87f, -1.68f);
        
        private readonly Vector3 _projectileDoubleUp01LStartPoint = new(-1.93f, -2.87f, -1.68f);
        private readonly Vector3 _projectileDoubleUp01LEndPoint = new(2.46f, -1.69f, 0.28f);
        private readonly Vector3 _projectileDoubleUp01RStartPoint = new(1.93f, -2.87f, -1.68f);
        private readonly Vector3 _projectileDoubleUp01REndPoint = new(-2.46f, -1.69f, 0.28f);
        
        private readonly Vector3 _projectileDoubleUp02LStartPoint = new(2.67f, 1.04f, 0.56f);
        private readonly Vector3 _projectileDoubleUp02LEndPoint = new(-3.12f, 2.43f, 0.6f);
        private readonly Vector3 _projectileDoubleUp02RStartPoint = new(-2.67f, 1.04f, 0.56f);
        private readonly Vector3 _projectileDoubleUp02REndPoint = new(3.12f, 2.43f, 0.6f);
        
        private readonly Vector3 _projectileTopAttackLStartPoint = new(-2.71f, 1.61f, 1.27f);
        private readonly Vector3 _projectileTopAttackLEndPoint = new(2.92f, 3.13f, 1.65f);
        private readonly Vector3 _projectileTopAttackRStartPoint = new(2.71f, 1.61f, 1.27f);
        private readonly Vector3 _projectileTopAttackREndPoint = new(-2.92f, 3.13f, 1.65f);
 
        [SerializeField] private MegaspiderMovement _movement;
        [SerializeField] private MegaspiderSpiderweb _spiderweb1;
        [SerializeField] private MegaspiderSpiderweb _spiderweb2;
        [SerializeField] private MegaspiderSpiderweb _spiderweb3;
        [SerializeField] private MegaspiderSpiderweb _spiderweb4;
        [SerializeField] private MegaspiderSpiderweb _spiderweb5;
        [SerializeField] private MegaspiderSpiderweb _spiderweb6;


        public void Initialize()
        {
            _movement.Bridge1Called += OnBridge1Called;
            _movement.Bridge1Broken += OnBridge1Broken;
            _movement.Bridge2Called += OnBridge2Called;
            _movement.Bridge2Broken += OnBridge2Broken;
            _movement.HangStartRequested += OnHangStartRequested;
            _movement.HangStopRequested += OnHangStopRequested;
            _movement.HangBreakRequested += OnHangBreakRequested;
        }

        private void OnDestroy()
        {
            _movement.Bridge1Called -= OnBridge1Called;
            _movement.Bridge1Broken -= OnBridge1Broken;
            _movement.Bridge2Called -= OnBridge2Called;
            _movement.Bridge2Broken -= OnBridge2Broken;
            _movement.HangStartRequested -= OnHangStartRequested;
            _movement.HangStopRequested -= OnHangStopRequested;
            _movement.HangBreakRequested -= OnHangBreakRequested;
        }

        private void OnBridge1Called(Type state)
        {
            if (state == typeof(MegaspiderEnterLState))
            {
                _spiderweb1.StartShoot(_enter01LStartPoint, _enter01LEndPoint);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb1.StartShoot(_enter01RStartPoint, _enter01REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileBottomAttackLState))
            {
                _spiderweb1.StartShoot(_projectileBottomAttackLStartPoint, _projectileBottomAttackLEndPoint);          
            }
            else if (state == typeof(MegaspiderProjectileBottomAttackRState))
            {
                _spiderweb1.StartShoot(_projectileBottomAttackRStartPoint, _projectileBottomAttackREndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackLState))
            {
                _spiderweb1.StartShoot(_projectileDoubleDown01LStartPoint, _projectileDoubleDown01LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackRState))
            {
                _spiderweb1.StartShoot(_projectileDoubleDown01RStartPoint, _projectileDoubleDown01REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackLState))
            {
                _spiderweb1.StartShoot(_projectileDoubleUp01LStartPoint, _projectileDoubleUp01LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackRState))
            {
                _spiderweb1.StartShoot(_projectileDoubleUp01RStartPoint, _projectileDoubleUp01REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileTopAttackLState))
            {
                _spiderweb1.StartShoot(_projectileTopAttackLStartPoint, _projectileTopAttackLEndPoint);          
            }
            else if (state == typeof(MegaspiderProjectileTopAttackRState))
            {
                _spiderweb1.StartShoot(_projectileTopAttackRStartPoint, _projectileTopAttackREndPoint);          
            }
        }

        private void OnBridge1Broken()
        {
            _spiderweb1.StartBreak();
        }

        private void OnBridge2Called(Type state)
        {
            if (state == typeof(MegaspiderEnterLState))
            {
                _spiderweb2.StartShoot(_enter02LStartPoint, _enter02LEndPoint);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb2.StartShoot(_enter02RStartPoint, _enter02REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackLState))
            {
                _spiderweb2.StartShoot(_projectileDoubleDown02LStartPoint, _projectileDoubleDown02LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleDownAttackRState))
            {
                _spiderweb2.StartShoot(_projectileDoubleDown02RStartPoint, _projectileDoubleDown02REndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackLState))
            {
                _spiderweb2.StartShoot(_projectileDoubleUp02LStartPoint, _projectileDoubleUp02LEndPoint);
            }
            else if (state == typeof(MegaspiderProjectileDoubleUpAttackRState))
            {
                _spiderweb2.StartShoot(_projectileDoubleUp02RStartPoint, _projectileDoubleUp02REndPoint);
            }
        }

        private void OnBridge2Broken()
        {
            _spiderweb2.StartBreak();
        }

        private void OnHangStartRequested(Type state, IPositionProvider endPositionProvider)
        {
            if (state == typeof(MegaspiderEnterLState))
            {
                _spiderweb6.StartHang(_enterHangLStartPoint, endPositionProvider);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb6.StartHang(_enterHangRStartPoint, endPositionProvider);          
            }
        }

        private void OnHangStopRequested()
        {
            _spiderweb6.StopHang();
        }

        private void OnHangBreakRequested()
        {
            _spiderweb6.StartBreakHang();
        }
    }
}
