using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderFallState : EnemyMovementStateBase
    {
        private const float InitialOutForceMagnitude = 1.8f;
        private const float OutForceIncrement = 5f;
        private const float FallForceIncrement = 6f;
        
        private Vector3 _initialDirection;
        private float _outForceMagnitude;
        private float _fallForceMagnitude;
        private bool _isFreeFall;
        
        
        
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private Transform _lampTransform;
        private float _exitYCoordinate;

        public MegaSpiderFallState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform,
            float exitYCoordinate,
            bool isFreeFall
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _exitYCoordinate = exitYCoordinate;
            _isFreeFall = isFreeFall;
        }
        
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
                
            _visibleBodyTransform.SetParent(null);
            
            _calculatedTransform.position = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_calculatedTransform);
            _visibleBodyTransform.localPosition = Vector3.zero;
            
            
            _initialDirection = (_calculatedTransform.position - _lampTransform.position).normalized;
            _outForceMagnitude = InitialOutForceMagnitude;
            _fallForceMagnitude = 0f;

        }

        public override void Tick()
        {
            if (_isFreeFall)
            {
                _calculatedTransform.position += Vector3.down * (_fallForceMagnitude * Time.deltaTime);
            }
            else
            {
                _calculatedTransform.position += _initialDirection * (_outForceMagnitude * Time.deltaTime) 
                                                 + Vector3.down * (_fallForceMagnitude * Time.deltaTime);    
            }
                
            _outForceMagnitude -= OutForceIncrement * Time.deltaTime;
            _outForceMagnitude = Mathf.Clamp(_outForceMagnitude, 0f, InitialOutForceMagnitude);
            
            _fallForceMagnitude += FallForceIncrement * Time.deltaTime;
            
            if (_calculatedTransform.position.y < _exitYCoordinate)
            {
                Debug.Log("MegaSpider FallState: Exiting");
                IsReadyToSwitch = true;
            }
        }
    }
}
