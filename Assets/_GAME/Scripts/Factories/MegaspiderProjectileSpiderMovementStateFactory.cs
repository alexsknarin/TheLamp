using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MegaspiderProjectileSpiderMovementStateFactory
    {
        // Dependencies
        private Transform _bodyTransform;
        private Transform _lampTransform;
        private Transform _rootTransform;
        
        // Constructor
        // Set Dependencies 
        public void SetEnemyDependencies(Transform bodyTransform, Transform lampTransform, Transform rootTransform)
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
            _rootTransform = rootTransform;       
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(MegaspiderProjectileSpiderIdleState))
            {
                return new MegaspiderProjectileSpiderIdleState();
            }
            else if (stateType == typeof(MegaspiderProjectileSpiderPreAttackState))
            {
                return new MegaspiderProjectileSpiderPreAttackState(_bodyTransform);
            }
            else if (stateType == typeof(MegaspiderProjectileSpiderAttackState))
            {
                return new MegaspiderProjectileSpiderAttackState(_bodyTransform, _lampTransform, _rootTransform);
            }
            else if (stateType == typeof(MegaspiderProjectileSpiderBounceState))
            {
                return new MegaspiderProjectileSpiderBounceState(_bodyTransform, _lampTransform);
            }
            else if (stateType == typeof(MegaspiderProjectileSpiderFallState))
            {
                return new MegaspiderProjectileSpiderFallState(_bodyTransform, _lampTransform, -4f, false);
            }
            
            return null;
        }
    }
}
