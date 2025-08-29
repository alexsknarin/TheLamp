using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpiderProjectileSpider.MovementStates
{
    public class MegaSpiderProjectileSpiderAttackState : EnemyMovementStateBase
    {
        private const float TransitionDuration = 0.33f;
        private const float Speed = 5f;
        private const float Gravity = 3.5f; // TODO: - remove if on top

        private float _localTime;
        private float _gravityMagnitude;
        private float _startDistance;
        private Vector3 _prevPos;
        private float _currentGravity;
        


        // Dependencies
        private Transform _bodyTransform;
        private Transform _lampTransform;

        public MegaSpiderProjectileSpiderAttackState(
                Transform bodyTransform,
                Transform lampTransform
            )
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
        }


        public override void Enter()
        {
            _bodyTransform.SetParent(null);
            
            Vector3 currentPosition = _bodyTransform.position;
            Vector3 lampPosition = _lampTransform.position;
            
            _localTime = 0;
            _gravityMagnitude = 0;
            _currentGravity = Gravity;
            if (currentPosition.y > 0)
                _currentGravity = 0;
            
            _startDistance = Vector3.Distance(currentPosition, lampPosition) - 0.5f;
        
            // Shift Attack Aim Center
            float side = Mathf.Sign(currentPosition.x);
            float sideFraction = Mathf.Abs(currentPosition.x) / 1.4f; // TODO: take camera into consideration - should be in the screen space
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
            float phase = _localTime / TransitionDuration;
        
            if (phase > 1)
                phase = 1;
        
            phase = Mathf.Pow(phase, .65f);
            direction = Vector3.Lerp(Vector3.up, direction, phase);
        
            Vector3 newPosition = currentPosition;
            newPosition += direction * (Speed * Time.deltaTime) + Vector3.down * (_gravityMagnitude * Time.deltaTime);
            _gravityMagnitude += Gravity * Time.deltaTime;
        
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
