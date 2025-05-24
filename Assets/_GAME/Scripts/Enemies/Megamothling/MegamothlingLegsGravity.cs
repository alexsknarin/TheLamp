using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling
{
    public class MegamothlingLegsGravity : MonoBehaviour
    {
        [SerializeField] private Transform _legsTransform;
        [SerializeField] private Transform _bodyTransform;
        
        private void LateUpdate()
        {
            Vector3 forwardTarget = _legsTransform.position + _bodyTransform.forward;
            Vector3 up = Vector3.up;
           
            _legsTransform.LookAt(forwardTarget, up);
        }
    }
}
