using _GAME.Scripts.Enemies.Dragonfly.FMovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Dragonfly
{
    public class DragonflyPatrolAttackPositionProvider
    {
        private readonly DragonflyPatrolAttackZoneRanges _patrolAttackZonesL;
        private readonly DragonflyPatrolAttackZoneRanges _patrolAttackZonesR;
        private readonly DragonflyPatrolAttackZoneRangesData _patrolAttackZonesDataL = new();
        private readonly DragonflyPatrolAttackZoneRangesData _patrolAttackZonesDataR = new();
        private DragonflyPatrolAttackZoneRangesData _patrolAttackZonesData;
        private readonly Vector3 _tailAttackBasePositionL;
        private readonly Vector3 _tailAttackBasePositionR;
    
        public DragonflyPatrolAttackPositionProvider(
            DragonflyPatrolAttackZoneRanges patrolAttackZonesL,
            DragonflyPatrolAttackZoneRanges patrolAttackZonesR,
            Vector3 tailAttackBasePositionBase
        )
        {
            _patrolAttackZonesL = patrolAttackZonesL;
            _patrolAttackZonesR = patrolAttackZonesR;
        
            _patrolAttackZonesL.GetRanges(_patrolAttackZonesDataL);
            _patrolAttackZonesR.GetRanges(_patrolAttackZonesDataR);
            _patrolAttackZonesData = _patrolAttackZonesDataL;
        
            _tailAttackBasePositionL = tailAttackBasePositionBase;
            _tailAttackBasePositionR = tailAttackBasePositionBase;
            _tailAttackBasePositionR.x *= -1;
        }
    
        public Vector3 GenerateRandomPreAttackHeadPosition(IState movementState)
        {
            // Get ranges Data
            if (movementState is ILeft)
            {
                _patrolAttackZonesData = _patrolAttackZonesDataL;
            }
            else
            {
                _patrolAttackZonesData = _patrolAttackZonesDataR;
            }
        
            // Select one of three zones
            int zone = Random.Range(0, 3);
            Vector3 rangeMin;
            Vector3 rangeMax;
            switch (zone)
            {
                case 0:
                    // Front
                    rangeMin = _patrolAttackZonesData.FrontZoneMin;
                    rangeMax = _patrolAttackZonesData.FrontZoneMax;
                    break;
                case 1:
                    // Back L
                    rangeMin = _patrolAttackZonesData.BackLZoneMin;
                    rangeMax = _patrolAttackZonesData.BackLZoneMax;
                    break;
                case 2:
                    // Back R
                    rangeMin = _patrolAttackZonesData.BackRZoneMin;
                    rangeMax = _patrolAttackZonesData.BackRZoneMax;
                    break;
                default:
                    rangeMin = Vector3.zero;
                    rangeMax = Vector3.zero;
                    break;
            }
        
            // Generate random position inside a range
            Vector3 patrolAttackPosition = Vector3.zero;
            patrolAttackPosition.x = Random.Range(rangeMin.x, rangeMax.x);
            patrolAttackPosition.y = Random.Range(rangeMin.y, rangeMax.y);
            patrolAttackPosition.z = Random.Range(rangeMin.z, rangeMax.z);
            patrolAttackPosition.Normalize();
        
            Debug.DrawRay(Vector3.zero, patrolAttackPosition*2, Color.blue, 5f);
        
            patrolAttackPosition.y = 0;
            patrolAttackPosition.Normalize();
        
            return patrolAttackPosition;
        }
    
        public Vector3 GenerateRandomPreAttackTailPosition(IState movementState)
        {
            Vector3 attackPosition = Vector3.zero;
        
            if (movementState is IRight)
            {
                attackPosition = _tailAttackBasePositionL;
            
            }
            else
            {
                attackPosition = _tailAttackBasePositionR;
            }
        
            Debug.DrawRay(Vector3.zero, attackPosition*2, Color.red, 5f);
        
            attackPosition.y = 0;
            attackPosition.Normalize();

            return attackPosition;
        }
    }
}
