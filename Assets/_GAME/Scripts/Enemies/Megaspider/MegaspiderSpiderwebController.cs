using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
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
        
        private WaitForSeconds _fallShootDelay = new WaitForSeconds(0.25f);
        private Vector3 _swingPivot;
        private float _swingZoneSize;
        private bool _isInReturnState;

        private IAttackWiresProvider _attackWiresProvider;
        
        
        public void Construct(IGameConfigService configService)
        {
            _swingPivot = configService.GameConfig.MegaspiderSwingSwingPivot;
            _swingZoneSize = configService.GameConfig.MegaspiderSwingZoneSize;
        }

        public void Initialize()
        {
            _isInReturnState = false;
            
            _movement.StaticBridge1Called += OnStaticBridge1Called;
            _movement.StaticBridge1Broken += OnStaticBridge1Broken;
            _movement.StaticBridge2Called += OnStaticBridge2Called;
            _movement.StaticBridge2Broken += OnStaticBridge2Broken;
            _movement.StaticBridge3Called += OnStaticBridge3Called;
            _movement.StaticBridge3Broken += OnStaticBridge3Broken;
            _movement.HangStartRequested += OnHangStartRequested;
            _movement.HangStopRequested += OnHangStopRequested;
            _movement.HangBreakRequested += OnHangBreakHangRequested;
            _movement.TangleAttackStarted += OnTangleAttackStarted;
            _movement.TangleAttackEnded += OnTangleAttackEnded;
            _movement.SuccessFallOutForceCancelled += OnSuccessFallOutForceCancelled;
            _movement.FailFallOutForceCancelled += OnFailFallOutForceCancelled;
            _movement.ClimbStateEnded += OnClimbStateEnded;
            _movement.SwingStateEnded += OnSwingStateEnded;
            _movement.FallStateEnded += OnFallStateEnded;
            _movement.WireAttackStateEntered += OnWireAttackStateEntered;
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
            _movement.TangleAttackStarted -= OnTangleAttackStarted;
            _movement.TangleAttackEnded -= OnTangleAttackEnded;
            _movement.SuccessFallOutForceCancelled -= OnSuccessFallOutForceCancelled;
            _movement.FailFallOutForceCancelled -= OnFailFallOutForceCancelled;
            _movement.ClimbStateEnded -= OnClimbStateEnded;
            _movement.SwingStateEnded -= OnSwingStateEnded;
            _movement.FallStateEnded -= OnFallStateEnded;
            _movement.WireAttackStateEntered -= OnWireAttackStateEntered;
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
                _spiderweb1.StartShootStatic(_config.ZigzagAttack01LStartPoint, _config.ZigzagAttack01LEndPoint, .15f, .5f);          
            }
            else if (state == typeof(MegaspiderZigzagAttackRState))
            {
                _spiderweb1.StartShootStatic(_config.ZigzagAttack01RStartPoint, _config.ZigzagAttack01REndPoint, 0.15f, 0.5f);          
            }
            else if (state == typeof(MegaspiderHangJumpAttackLState))
            {
                _spiderweb1.StartShootStatic(_config.HangJumpAttackLStartPoint, _config.HangJumpAttackLEndPoint);          
            }
            else if (state == typeof(MegaspiderHangJumpAttackRState))
            {
                _spiderweb1.StartShootStatic(_config.HangJumpAttackRStartPoint, _config.HangJumpAttackREndPoint);          
            }
        }

        private void OnStaticBridge1Broken()
        {
            _spiderweb1.StartFallBreakStatic();
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
                _spiderweb2.StartShootStatic(_config.ZigzagAttack02LStartPoint, _config.ZigzagAttack02LEndPoint, 0.15f, 0.5f);          
            }
            else if (state == typeof(MegaspiderZigzagAttackRState))
            {
                _spiderweb2.StartShootStatic(_config.ZigzagAttack02RStartPoint, _config.ZigzagAttack02REndPoint, 0.15f, 0.5f);
            }
        }

        private void OnStaticBridge2Broken()
        {
            _spiderweb2.StartFallBreakStatic();
        }

        private void OnStaticBridge3Called(Type state)
        {
            if (state == typeof(MegaspiderZigzagAttackLState))
            {
                _spiderweb3.StartShootStatic(_config.ZigzagAttack03LStartPoint, _config.ZigzagAttack03LEndPoint, 0.15f, 0.5f);          
            }
            else if (state == typeof(MegaspiderZigzagAttackRState))
            {
                _spiderweb3.StartShootStatic(_config.ZigzagAttack03RStartPoint, _config.ZigzagAttack03REndPoint, 0.15f, 0.5f);          
            }
        }

        private void OnStaticBridge3Broken()
        {
            _spiderweb3.StartFallBreakStatic();
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
            if (state == typeof(MegaspiderHangJumpAttackLState))
            {
                _spiderweb6.StartHang(_config.HangJumpAttackLHangPoint, endPositionProvider);          
            }
            if (state == typeof(MegaspiderHangJumpAttackRState))
            {
                _spiderweb6.StartHang(_config.HangJumpAttackRHangPoint, endPositionProvider);          
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

        private void OnTangleAttackStarted(ITangledWireProvider positionProvider)
        {
            _spiderweb6.StartTangle(positionProvider);
        }

        private void OnTangleAttackEnded()
        {
            _spiderweb6.StartDetangle();
        }

        private void OnSuccessFallOutForceCancelled(IPositionProvider positionProvider)
        {
            _isInReturnState = true;
            Vector3 hangPoint = FindFallSwingHangPoint(positionProvider);
            _spiderweb6.StartShootDynamic(positionProvider, hangPoint);
        }

        private void OnClimbStateEnded()
        {
            _spiderweb6.StopHang();
            _isInReturnState = false;
        }

        private void OnSwingStateEnded()
        {
            _spiderweb6.StopHang();
            _isInReturnState = false;
        }

        private void OnFallStateEnded(IPositionProvider positionProvider)
        {
            if (!_isInReturnState)
            {
                Vector3 hangPoint = FindFallSwingHangPoint(positionProvider);
                _spiderweb6.StartShootDynamic(positionProvider, hangPoint);
                _isInReturnState = true;
            }
        }

        private void OnFailFallOutForceCancelled(IPositionProvider positionProvider)
        {
            _isInReturnState = true;
            StartCoroutine(DelayFallSpiderwebShoot(positionProvider));
        }

        private IEnumerator DelayFallSpiderwebShoot(IPositionProvider positionProvider)
        {
            yield return _fallShootDelay;
            Vector3 hangPoint = FindFallSwingHangPoint(positionProvider);
            _spiderweb6.StartShootDynamic(positionProvider, hangPoint);
        }

        private Vector3 FindFallSwingHangPoint(IPositionProvider positionProvider)
        {
            Vector3 hangPoint;
            if (Mathf.Abs(positionProvider.Position3D.x) > _swingZoneSize)
            {
                hangPoint = positionProvider.Position3D;
                hangPoint.y = 4f;
            }
            else
            {
                hangPoint = _swingPivot;
                hangPoint.x = Mathf.Abs(hangPoint.x) * Mathf.Sign(positionProvider.Position3D.x);
            }

            return hangPoint;
        }

        private void OnWireAttackStateEntered(IAttackWiresProvider attackWiresProvider)
        {
            _attackWiresProvider = attackWiresProvider;
            _attackWiresProvider.AttackWires[0].Activated += OnWire1Activated;
            _attackWiresProvider.AttackWires[1].Activated += OnWire2Activated;
            _attackWiresProvider.AttackWires[2].Activated += OnWire3Activated;
            _attackWiresProvider.AttackWires[3].Activated += OnWire4Activated;
            _attackWiresProvider.AttackWires[4].Activated += OnWire5Activated;

            _attackWiresProvider.AttackWires[0].Destroyed += OnWire1Deactivated;
            _attackWiresProvider.AttackWires[0].Deactivated += OnWire1Deactivated;
            _attackWiresProvider.AttackWires[1].Destroyed += OnWire2Deactivated;
            _attackWiresProvider.AttackWires[1].Deactivated += OnWire2Deactivated;
            _attackWiresProvider.AttackWires[2].Destroyed += OnWire3Deactivated;
            _attackWiresProvider.AttackWires[2].Deactivated += OnWire3Deactivated;
            _attackWiresProvider.AttackWires[3].Destroyed += OnWire4Deactivated;
            _attackWiresProvider.AttackWires[3].Deactivated += OnWire4Deactivated;
            _attackWiresProvider.AttackWires[4].Destroyed += OnWire5Deactivated;
            _attackWiresProvider.AttackWires[4].Deactivated += OnWire5Deactivated;
        }

        private void OnWire1Activated()
        {
            _spiderweb1.StartShootDynamicFull(_attackWiresProvider.AttackWires[0]);
        }

        private void OnWire2Activated()
        {
            _spiderweb2.StartShootDynamicFull(_attackWiresProvider.AttackWires[1]);
        }

        private void OnWire3Activated()
        {
            _spiderweb3.StartShootDynamicFull(_attackWiresProvider.AttackWires[2]);
        }

        private void OnWire4Activated()
        {
            _spiderweb4.StartShootDynamicFull(_attackWiresProvider.AttackWires[3]);
        }

        private void OnWire5Activated()
        {
            _spiderweb5.StartShootDynamicFull(_attackWiresProvider.AttackWires[4]);
        }

        private void OnWire1Deactivated()
        {
            _attackWiresProvider.AttackWires[0].Activated -= OnWire1Activated;
            _attackWiresProvider.AttackWires[0].Destroyed -= OnWire1Deactivated;
            _attackWiresProvider.AttackWires[0].Deactivated -= OnWire1Deactivated;
            _spiderweb1.StartFallBreakDynamic();
        }
        
        private void OnWire2Deactivated()
        {
            _attackWiresProvider.AttackWires[1].Activated -= OnWire2Activated;
            _attackWiresProvider.AttackWires[1].Destroyed -= OnWire2Deactivated;
            _attackWiresProvider.AttackWires[1].Deactivated -= OnWire2Deactivated;
            _spiderweb2.StartFallBreakDynamic();
        }
        
        private void OnWire3Deactivated()
        {
            _attackWiresProvider.AttackWires[2].Activated -= OnWire3Activated;
            _attackWiresProvider.AttackWires[2].Destroyed -= OnWire3Deactivated;
            _attackWiresProvider.AttackWires[2].Deactivated -= OnWire3Deactivated;
            _spiderweb3.StartFallBreakDynamic();
        }
        
        private void OnWire4Deactivated()
        {
            _attackWiresProvider.AttackWires[3].Activated -= OnWire4Activated;
            _attackWiresProvider.AttackWires[3].Destroyed -= OnWire4Deactivated;
            _attackWiresProvider.AttackWires[3].Deactivated -= OnWire4Deactivated;
            _spiderweb4.StartFallBreakDynamic();
        }
        
        private void OnWire5Deactivated()
        {
            _attackWiresProvider.AttackWires[4].Activated -= OnWire5Activated;
            _attackWiresProvider.AttackWires[4].Destroyed -= OnWire5Deactivated;
            _attackWiresProvider.AttackWires[4].Deactivated -= OnWire5Deactivated;
            _spiderweb5.StartFallBreakDynamic();
        }
    }
}
