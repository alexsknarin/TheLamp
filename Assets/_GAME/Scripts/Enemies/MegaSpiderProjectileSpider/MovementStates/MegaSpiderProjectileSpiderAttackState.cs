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
                _currentGravity *= 0.25f;
            
            _startDistance = Vector3.Distance(currentPosition, lampPosition) - 0.5f;
        
            // Shift Attack Aim Center
            float side = Mathf.Sign(currentPosition.x);
            float sideFraction = Mathf.Abs(currentPosition.x) / 1.4f; 
            float lampShift = Mathf.Lerp(0.0f, 0.36f, sideFraction) * side;
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
        
            phase = Mathf.Pow(phase, .65f);
            direction = Vector3.Lerp(Vector3.up, direction, phase);
        
            Vector3 newPosition = currentPosition;
            newPosition += direction * (_speed * Time.deltaTime) + Vector3.down * (_gravityMagnitude * Time.deltaTime);
            _gravityMagnitude += _currentGravity * Time.deltaTime;
        
            // Use Distance to collision and Lerp
            float distance = Vector3.Distance(newPosition, lampPosition) - 0.5f;
            float zPhase = Mathf.Clamp01(1 - distance / _startDistance);
            newPosition.z = Mathf.Lerp(currentPosition.z, 0f, zPhase);
        
            _bodyTransform.position = newPosition;
        
            _localTime += Time.deltaTime;
        
            Debug.DrawLine(_prevPos, newPosition, Color.cyan, 5f);
        }
    }
}
