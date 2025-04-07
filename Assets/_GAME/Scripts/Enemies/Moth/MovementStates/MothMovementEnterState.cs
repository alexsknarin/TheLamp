using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth.MovementStates
{
    public class MothMovementEnterState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly float _speed;
        private readonly float _radius;
        private readonly float _verticalAmplitude;
    
        // State specific attributes
        private readonly float _deadZoneAngle = 35;
        private readonly float _minDistance = 2.5f;
        private readonly float _maxDistance = 4.7f;

        private readonly float _depthMultiplier = 1.6f;
        private readonly float _noiseFrequency = 9f;
        private readonly float _noiseAmplitude = 0.05f;
        private Vector2 _endPos = Vector2.zero;
        private Vector2 _enterDirection;
        private float _initialDistance;
        private float _phase;


        public MothMovementEnterState(
            Vector3 cameraPosition,
            float speed,
            float radius,
            float verticalAmplitude
        )
        {
            _cameraPosition = cameraPosition;
            _speed = speed;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
        }
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
            Position2D = GenerateSpawnPosition(_deadZoneAngle, _minDistance, _maxDistance);
        
            CalculateEndPos();

            _enterDirection = (Vector2.zero - Position2D).normalized;
            _initialDistance = (_endPos - Position2D).magnitude;
            _phase = 1;
        }

        private void CalculateEndPos()
        {
            var denominator = FindIntersectionWithEllipse();
            _endPos = Vector3.zero;
            _endPos.x = denominator * Position2D.x;
            _endPos.y = denominator * Position2D.y;
        }

        private float FindIntersectionWithEllipse()
        {
            float a = _radius;
            float b = _radius * _verticalAmplitude;
            float denominator = ((a * b) / (Mathf.Sqrt((a * a) * (Position2D.y * Position2D.y) 
                                                       + (b * b) * (Position2D.x * Position2D.x))));
            return denominator;
        }

        public override void Tick()
        {
        
            Position2D += _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
            _phase = (_endPos - Position2D).magnitude / _initialDistance;
        
            // Add noise
            Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
            Position2D += trajectoryNoise * _noiseAmplitude;

            // Depth To Camera
            float distancePhase = 1 - (_endPos - Position2D).magnitude / _initialDistance;
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            DepthDirection = cameraDirection * (Mathf.Lerp(Position2D.y * _depthMultiplier, Position2D.y, distancePhase));
    

            if(_phase < 0.02f || Position2D.magnitude < _radius)
            {
                IsReadyToSwitch = true;
            }
        }

        // TODO: make Appear from the top more often
        private Vector2 GenerateSpawnPosition(float deadZoneAngle, float minDistance, float maxDistance)
        {
            Vector3 spawnPosition = Vector3.up;
            float angle = deadZoneAngle + Random.Range(0, 360-deadZoneAngle*2);
        
            spawnPosition = Quaternion.AngleAxis(angle, Vector3.forward) * spawnPosition;
        
            float distance = Mathf.Lerp(minDistance, maxDistance, Mathf.Abs(spawnPosition.y));
            spawnPosition *= distance;
            
            return spawnPosition;
        }
    }
}
