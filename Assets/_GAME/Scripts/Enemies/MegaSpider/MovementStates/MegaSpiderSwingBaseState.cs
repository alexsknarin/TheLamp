using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSwingBaseState : EnemyMovementStateBase
    {
        private Vector3 _initialDirection;
        private float _swingForceMagnitude;
        private float _swingDownForceMagnitude;
        private float _fallForceMagnitude;
        private bool _isFreeFall;
        private Vector3 _swingDirection;
        

        private float _localTime;
        private Vector3 _currentSwingPivot;
        
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private Transform _rootTransform;
        private bool _isLeftSide;
        // Config
        private readonly float _overallSpedFactor;
        private readonly float _initialFallForceMagnitude;
        private readonly float _swingForceIncrement;
        private readonly float _fallForceIncrement;
        private readonly float _swingDownIncrement;
        private readonly float _exitDistance;
        private readonly Vector3 _swingPivot;
        
        public MegaspiderSwingBaseState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,
            Transform rootTransform,
            IGameConfigService configService,
            bool isLeftSide
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _rootTransform = rootTransform;
            _isLeftSide = isLeftSide;

            _overallSpedFactor = configService.GameConfig.MegaspiderSwingOverallSpedFactor;
            _initialFallForceMagnitude = configService.GameConfig.MegaspiderSwingInitialFallForceMagnitude;
            _swingForceIncrement = configService.GameConfig.MegaspiderSwingSwingForceIncrement;
            _fallForceIncrement = configService.GameConfig.MegaspiderSwingFallForceIncrement;
            _swingDownIncrement = configService.GameConfig.MegaspiderSwingSwingDownIncrement;
            _exitDistance = configService.GameConfig.MegaspiderSwingExitDistance;
            _swingPivot = configService.GameConfig.MegaspiderSwingSwingPivot;
        }
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            _visibleBodyTransform.SetParent(_rootTransform);
            
            _calculatedTransform.position = _visibleBodyTransform.position;
            HierarchyUtilities.ParentWithoutOffset(_visibleBodyTransform, _calculatedTransform);
            
            _currentSwingPivot = _swingPivot;
            if (!_isLeftSide)
                _currentSwingPivot.x *= -1;
            
            DrawSwingDebugLine();
            
            _initialDirection = Vector3.down;
            
            _fallForceMagnitude = _initialFallForceMagnitude;
            _swingForceMagnitude = 0f;
            _swingDownForceMagnitude = 0f;

            if (_isLeftSide)
            {
                _swingDirection = Vector3.left;
            }
            else
            {
                _swingDirection = Vector3.right;
            }
        }

        public override void Tick()
        {
            _calculatedTransform.position += _initialDirection * (_fallForceMagnitude * Time.deltaTime * _overallSpedFactor)
                + _swingDirection * (_swingForceMagnitude * Time.deltaTime * _overallSpedFactor)
                + Vector3.down * (_swingDownForceMagnitude * Time.deltaTime * _overallSpedFactor);
            
            _fallForceMagnitude -= _fallForceIncrement * Time.deltaTime;
            _fallForceMagnitude = Mathf.Clamp(_fallForceMagnitude, 0f, _initialFallForceMagnitude);
            
            _swingForceMagnitude += _swingForceIncrement * Time.deltaTime;
            _swingDownForceMagnitude += _swingDownIncrement * Time.deltaTime;
            DrawSwingDebugLine();
            
            if (_isLeftSide && _calculatedTransform.position.x < -_exitDistance)
            {
                IsReadyToSwitch = true;
            }
            else if (!_isLeftSide && _calculatedTransform.position.x > _exitDistance)
            {
                IsReadyToSwitch = true;
            }
        }

        private void DrawSwingDebugLine()
        {
            Debug.DrawLine(
                _calculatedTransform.position, 
                _currentSwingPivot,
                Color.red
            );
        } 
    }
}
