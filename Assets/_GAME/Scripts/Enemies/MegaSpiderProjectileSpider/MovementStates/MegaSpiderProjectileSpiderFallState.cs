using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpiderProjectileSpider.MovementStates
{
    public class MegaSpiderProjectileSpiderFallState : EnemyMovementStateBase
    {
        // TODO: Extract fall logic into a static class Methods - move to library
        
        private const float InitialOutForceMagnitude = 1.8f;
        private const float OutForceIncrement = 5f;
        private const float FallForceIncrement = 6f;
        
        private Vector3 _initialDirection;
        private float _outForceMagnitude;
        private float _fallForceMagnitude;
        private bool _isFreeFall;
        
        
        
        // Dependencies
        private Transform _bodyTransform;
        private Transform _lampTransform;
        private float _exitYCoordinate;

        public MegaSpiderProjectileSpiderFallState(
            Transform bodyTransform, 
            Transform lampTransform,
            float exitYCoordinate,
            bool isFreeFall
            )
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
            _exitYCoordinate = exitYCoordinate;
            _isFreeFall = isFreeFall;
        }
        
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            
            _initialDirection = (_bodyTransform.position - _lampTransform.position).normalized;
            _outForceMagnitude = InitialOutForceMagnitude;
            _fallForceMagnitude = 0f;

        }

        public override void Tick()
        {
            if (_isFreeFall)
            {
                _bodyTransform.position += Vector3.down * (_fallForceMagnitude * Time.deltaTime);
            }
            else
            {
                _bodyTransform.position += _initialDirection * (_outForceMagnitude * Time.deltaTime) 
                                           + Vector3.down * (_fallForceMagnitude * Time.deltaTime);    
            }
                
            _outForceMagnitude -= OutForceIncrement * Time.deltaTime;
            _outForceMagnitude = Mathf.Clamp(_outForceMagnitude, 0f, InitialOutForceMagnitude);
            
            _fallForceMagnitude += FallForceIncrement * Time.deltaTime;
            
            if (_bodyTransform.position.y < _exitYCoordinate)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
