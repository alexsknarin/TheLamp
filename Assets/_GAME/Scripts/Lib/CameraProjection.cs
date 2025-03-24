using UnityEngine;

namespace _GAME.Scripts.Lib
{
    public class CameraProjection
    {
        public static Vector3 ProjectPointOnXYPlane(Vector3 cameraPoint, Vector3 targetPoint)
        {
            float katetLength = Mathf.Abs(targetPoint.z);
            Vector3 hippotenuseDirection = (targetPoint - cameraPoint).normalized;
            float angleCos = Vector3.Dot(Vector3.forward, hippotenuseDirection);
            Vector3 projectedPoint = targetPoint + hippotenuseDirection * (katetLength / angleCos);
            projectedPoint.z = 0;
            return projectedPoint;
        }
    }
}
