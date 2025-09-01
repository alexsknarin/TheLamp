using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.MegaSpiderProjectileSpider.MovementStates;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MegaSpiderProjectileSpiderMovementStateFactory
    {
        // Dependencies
        private Transform _bodyTransform;
        private Transform _lampTransform;
        
        // Constructor
        // Set Dependencies 
        public void SetEnemyDependencies(Transform bodyTransform, Transform lampTransform)
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(MegaSpiderProjectileSpiderIdleState))
            {
                return new MegaSpiderProjectileSpiderIdleState();
            }
            else if (stateType == typeof(MegaSpiderProjectileSpiderPreAttackState))
            {
                return new MegaSpiderProjectileSpiderPreAttackState(_bodyTransform);
            }
            else if (stateType == typeof(MegaSpiderProjectileSpiderAttackState))
            {
                return new MegaSpiderProjectileSpiderAttackState(_bodyTransform, _lampTransform);
            }
            else if (stateType == typeof(MegaSpiderProjectileSpiderBounceState))
            {
                return new MegaSpiderProjectileSpiderBounceState(_bodyTransform, _lampTransform);
            }
            else if (stateType == typeof(MegaSpiderProjectileSpiderFallState))
            {
                return new MegaSpiderProjectileSpiderFallState(_bodyTransform, _lampTransform, -4f, false);
            }
            
            return null;
        }
    }
}
