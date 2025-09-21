using System;
using _GAME.Scripts.Enemies.Megaspider.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderSpiderwebController : MonoBehaviour, IInitializable
    {
        private readonly Vector3 enter01LStartPoint = new Vector3(-4.74f, 2.61f, 7.15f);
        private readonly Vector3 enter01LEndPoint = new Vector3(2.71f, 1.76f, 1.76f);
        private readonly Vector3 enter01RStartPoint = new Vector3(4.74f, 2.61f, 7.15f);
        private readonly Vector3 enter01REndPoint = new Vector3(-2.71f, 1.76f, 1.76f);
        
        private readonly Vector3 enter02LStartPoint = new Vector3(2.71f, 1.55f, 0.69f);
        private readonly Vector3 enter02LEndPoint = new Vector3(-1.9f, 1.76f, -1.2f);
        private readonly Vector3 enter02RStartPoint = new Vector3(-2.71f, 1.55f, 0.69f);
        private readonly Vector3 enter02REndPoint = new Vector3(1.9f, 1.76f, -1.2f);
        
        private readonly Vector3 enterHangLStartPoint = new Vector3(-0.35f, 1.05f, -3.99f);
        private readonly Vector3 enterHangRStartPoint = new Vector3(0.35f, 1.05f, -3.99f);
        
        
        
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
                _spiderweb1.StartShoot(enter01LStartPoint, enter01LEndPoint);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb1.StartShoot(enter01RStartPoint, enter01REndPoint);
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
                _spiderweb2.StartShoot(enter02LStartPoint, enter02LEndPoint);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb2.StartShoot(enter02RStartPoint, enter02REndPoint);
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
                _spiderweb6.StartHang(enterHangLStartPoint, endPositionProvider);          
            }
            else if (state == typeof(MegaspiderEnterRState))
            {
                _spiderweb6.StartHang(enterHangRStartPoint, endPositionProvider);          
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
