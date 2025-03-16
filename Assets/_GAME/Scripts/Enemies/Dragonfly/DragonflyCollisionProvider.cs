using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly
{
    public class DragonflyCollisionProvider : MonoBehaviour
    {
        [SerializeField] private List<Transform> _collisionTransforms;
        [SerializeField] private List<float> _collisionRadii;
        private int _currentIndex = 0;
    
        public Vector2 CurrentCollisionPoint => _collisionTransforms[_currentIndex].position;
        public float CurrentCollisionRadius => _collisionRadii[_currentIndex];
        public Transform CurrentCollisionTransform => _collisionTransforms[_currentIndex];

        public void FindClosestPointIndex()
        {
            float minDistance = float.MaxValue;
            int minIndex = -1;
            for (int i = 0; i < _collisionTransforms.Count; i++)
            {
                float distance = ((Vector2)_collisionTransforms[i].position).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }
            _currentIndex = minIndex;
        
            Debug.Log(" ++++ Closest point index: " + _currentIndex);
        }
    
        private void OnDrawGizmos()
        {
            for (int i = 0; i < _collisionTransforms.Count; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(_collisionTransforms[i].position, _collisionRadii[i]);
            }
        
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, _collisionTransforms[_currentIndex].position);
        }
    }
}
