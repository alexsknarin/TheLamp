using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates
{
    public class MegaspiderProjectileSpiderAttackState : EnemyMovementStateBase
    {
        private float _localTime;
        private float _gravityMagnitude;
        private float _startDistance;
        private Vector3 _prevPos;
        private float _currentGravity;

        // Dependencies
        private Transform _bodyTransform;
        private Transform _lampTransform;
        private Transform _rootTransform;
        
        private readonly float _transitionDuration;
        private readonly float _speed;
        private readonly float _gravity;
        private readonly float _lampRadius;
        private readonly float _lampSideMaximum;
        private readonly float _lampShiftMaximum;
        private readonly float _attackAccelerationPower;
        private readonly float _topGravityMultiplier;
        
        public MegaspiderProjectileSpiderAttackState(
                Transform bodyTransform,
                Transform lampTransform,
                Transform rootTransform,
                IGameConfigService configService
            )
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
            _rootTransform = rootTransform;

            _transitionDuration = configService.GameConfig.MegaspiderProjectileSpiderTransitionDuration;
            _speed = configService.GameConfig.MegaspiderProjectileSpiderSpeed;
            _gravity = configService.GameConfig.MegaspiderProjectileSpiderGravity;
            _lampRadius = configService.PlayerConfig.LampCollisionRadius;
            _lampSideMaximum = configService.GameConfig.MegaspiderProjectileSpiderLampSideMaximum;
            _lampShiftMaximum = configService.GameConfig.MegaspiderProjectileSpiderLampShiftMaximum;
            _attackAccelerationPower = configService.GameConfig.MegaspiderProjectileSpiderAttackAccelerationPower;
            _topGravityMultiplier = configService.GameConfig.MegaspiderProjectileSpiderAttackTopGravityMultiplier;
        }
        
        public override void Enter()
        {
            _bodyTransform.SetParent(_rootTransform);
            
            Vector3 currentPosition = _bodyTransform.position;
            Vector3 lampPosition = _lampTransform.position;
            
            _localTime = 0;
            _gravityMagnitude = 0;
            _currentGravity = _gravity;
            if (currentPosition.y > 0)
                _currentGravity *= _topGravityMultiplier;
            
            _startDistance = Vector3.Distance(currentPosition, lampPosition) - _lampRadius;
        
            // Shift Attack Aim Center
            float side = Mathf.Sign(currentPosition.x);
            float sideFraction = Mathf.Abs(currentPosition.x) / _lampSideMaximum; 
            float lampShift = Mathf.Lerp(0.0f, _lampShiftMaximum, sideFraction) * side;
            lampPosition.x += lampShift;
        
            Debug.DrawLine(Vector3.zero, lampPosition, Color.red, 55f);
        }

        public override void Tick()
        {
            Vector3 currentPosition = _bodyTransform.position;
            Vector3 lampPosition = _lampTransform.position;
            _prevPos = currentPosition;

            Vector3 direction = (lampPosition - currentPosition).normalized;
            float phase = _localTime / _transitionDuration;
        
            if (phase > 1)
                phase = 1;
        
            phase = Mathf.Pow(phase, _attackAccelerationPower);
            direction = Vector3.Lerp(Vector3.up, direction, phase);
        
            Vector3 newPosition = currentPosition;
            newPosition += direction * (_speed * Time.deltaTime) + Vector3.down * (_gravityMagnitude * Time.deltaTime);
            _gravityMagnitude += _currentGravity * Time.deltaTime;
        
            // Use Distance to collision and Lerp
            float distance = Vector3.Distance(newPosition, lampPosition) - _lampRadius;
            float zPhase = Mathf.Clamp01(1 - distance / _startDistance);
            newPosition.z = Mathf.Lerp(currentPosition.z, 0f, zPhase);
        
            _bodyTransform.position = newPosition;
        
            _localTime += Time.deltaTime;
        
            Debug.DrawLine(_prevPos, newPosition, Color.cyan, 5f);
        }
    }
}
