using UnityEngine;

namespace _GAME.Scripts.Lib
{
    public class CameraProjection
    {
        public static Vector3 ProjectPointOnXYPlane(Vector3 cameraPoint, Vector3 targetPoint)
        {
            float katetLength = Mathf.Abs(targetPoint.z);
            Vector3 hippotenuseDirection = (targetPoint - cameraPoint).normalized;
            Vector3 katetDirection = Vector3.forward;
            if (targetPoint.z > 0)
            {
                hippotenuseDirection *= -1;
                katetDirection = Vector3.back;
            }
            
            float angleCos = Vector3.Dot(katetDirection, hippotenuseDirection);
            Vector3 projectedPoint = targetPoint + hippotenuseDirection * (katetLength / angleCos);
            projectedPoint.z = 0;
            return projectedPoint;
        }
    }
}
