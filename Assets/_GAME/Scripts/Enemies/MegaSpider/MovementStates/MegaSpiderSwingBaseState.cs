using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSwingBaseState : EnemyMovementStateBase
    {
        private const float OverallSpedFactor = 1.5f;
        private const float InitialFallForceMagnitude = .9f;
        private const float SwingForceIncrement = 5f;
        private const float FallForceIncrement = 2f;
        private const float SwingDownIncrement = 1f;
        private const float ExitDistance = 4f;
        
        
        private readonly Vector3 _swingPivot = new Vector3(-2.35f, 2.7f, 0);
        
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

        public MegaspiderSwingBaseState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,
            Transform rootTransform,
            bool isLeftSide
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _rootTransform = rootTransform;
            _isLeftSide = isLeftSide;
        }
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            _visibleBodyTransform.SetParent(_rootTransform);
            
            // TODO: TEMP for tests - remove later
            Vector3 position = new Vector3(-0.16f, -1.91f, 0);
            _visibleBodyTransform.position = position;
            
            _calculatedTransform.position = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_calculatedTransform);
            _visibleBodyTransform.localPosition = Vector3.zero;
            
            _currentSwingPivot = _swingPivot;
            if (!_isLeftSide)
                _currentSwingPivot.x *= -1;
            
            DrawSwingDebugLine();
            
            _initialDirection = Vector3.down;
            
            _fallForceMagnitude = InitialFallForceMagnitude;
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
            _calculatedTransform.position += _initialDirection * (_fallForceMagnitude * Time.deltaTime * OverallSpedFactor)
                + _swingDirection * (_swingForceMagnitude * Time.deltaTime * OverallSpedFactor)
                + Vector3.down * (_swingDownForceMagnitude * Time.deltaTime * OverallSpedFactor);
            
            _fallForceMagnitude -= FallForceIncrement * Time.deltaTime;
            _fallForceMagnitude = Mathf.Clamp(_fallForceMagnitude, 0f, InitialFallForceMagnitude);
            
            _swingForceMagnitude += SwingForceIncrement * Time.deltaTime;
            _swingDownForceMagnitude += SwingDownIncrement * Time.deltaTime;
            DrawSwingDebugLine();
            
            if (_isLeftSide && _calculatedTransform.position.x < -ExitDistance)
            {
                IsReadyToSwitch = true;
            }
            else if (!_isLeftSide && _calculatedTransform.position.x > ExitDistance)
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
