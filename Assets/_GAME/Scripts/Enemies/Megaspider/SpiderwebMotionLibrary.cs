using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class SpiderwebMotionLibrary
    {
        public static bool CalculateWireShootMotion(
            Vector3 startPosition, 
            Vector3 endPosition, 
            LineRenderer lineRenderer,
            float duration,
            float sineFrequency,
            AnimationCurve shootSineAmplitudeCurve,
            ref float localTime)
        {
            bool isFinished = false;
            Vector3 axis = (endPosition - startPosition).normalized;
            Vector3 perpendicular = Vector3.Cross(axis, Vector3.back);
            
            float phase = localTime / duration;
            
            if (phase > 1)
            {
                isFinished = true;
                return isFinished;
            }
        
            Vector3 newEndPos = Vector3.Lerp(startPosition, endPosition, phase);

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(startPosition, newEndPos, resampledPhase);
                float u = 1 - resampledPhase;
                float displace = Mathf.Sin(u * sineFrequency * phase) 
                                 * shootSineAmplitudeCurve.Evaluate(phase);
                resampledPos += perpendicular * (displace * resampledPhase);
            
                lineRenderer.SetPosition(i, resampledPos);
            }
       
            localTime += Time.deltaTime;
            
            return isFinished;
        }

        public static bool CalculateWireVibrateMotion(
            Vector3 startPosition, 
            Vector3 endPosition, 
            LineRenderer lineRenderer,
            float duration,
            AnimationCurve vibrateFrequencyCurve,
            AnimationCurve vibrateAmplitudeCurve,
            float amplitude,
            ref float localTime)
        {
            bool isFinished = false;
            Vector3 axis = (endPosition - startPosition).normalized;
            Vector3 perpendicular = Vector3.Cross(axis, Vector3.back);
        
            float phase = localTime / duration;
        
            if (phase > 1)
            {
                isFinished = true;
                return isFinished;
            }
        
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(startPosition, endPosition, resampledPhase);
                float u = 1 - resampledPhase;
                float displace = Mathf.Sin(u * Mathf.PI) 
                                 * Mathf.Cos(phase * vibrateFrequencyCurve.Evaluate(phase))
                                 * vibrateAmplitudeCurve.Evaluate(phase)
                                 * amplitude;
                resampledPos += perpendicular * displace;
            
                lineRenderer.SetPosition(i, resampledPos);
            }
       
            localTime += Time.deltaTime;
            return isFinished;
        }

        public static bool CalculateWireFallBreakMotion(
            Vector3 startPosition, 
            Vector3 endPosition,
            LineRenderer lineRenderer,
            float duration,
            AnimationCurve fallContractionCurve,
            AnimationCurve fallDeformationCurve,
            ref float localTime)
        {
            bool isFinished = false;
            Vector3 axis = (endPosition - startPosition).normalized;
            Vector3 perpendicular = Vector3.Cross(axis, Vector3.back);
        
            // Calculate once
            float fullAngle = Vector3.Angle(axis, Vector3.down);
        
            float phase = localTime / duration;
            if (phase > 1)
            {
                isFinished = true;
                return isFinished;
            }
        
            float currentAngle = Mathf.Lerp(0, fullAngle, Mathf.Pow(phase, 3f));
            float bendDirection = 1;
            if (endPosition.x < startPosition.x)
            {
                currentAngle *= -1;
                bendDirection *= -1;
            }
        
            Vector3 newEndPos = endPosition - startPosition;
            Quaternion endRotation = Quaternion.AngleAxis(currentAngle, Vector3.back);
            newEndPos = endRotation * newEndPos;
            newEndPos *= fallContractionCurve.Evaluate(phase);
            perpendicular = endRotation * perpendicular;
            newEndPos += startPosition;
        
        
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(startPosition, newEndPos, resampledPhase);
                float u = 1 - resampledPhase;
                float displace = Mathf.Sin(u * 15f + phase * 10f) 
                                 * (1-u) 
                                 * fallDeformationCurve.Evaluate(phase) * .5f
                                 - Mathf.Sin(u * Mathf.PI) * phase * bendDirection;

                resampledPos += perpendicular * displace;
            
                lineRenderer.SetPosition(i, resampledPos);
            }
       
            localTime += Time.deltaTime;
            return isFinished;
        }

        public static bool CalculateWireBreakHangMotion(
            Vector3 startPosition, 
            Vector3 endPosition,
            LineRenderer lineRenderer,
            float duration,
            float breakHangNoiseOffset,
            ref float localTime
            )
        {
            bool isFinished = false;
            float phase = localTime / duration;
            if (phase > 1)
            {
                isFinished = true;
                return isFinished;
            }
            
            Vector3 endPos = Vector3.Lerp(endPosition, startPosition, phase);
                
            for(int i = 0; i < lineRenderer.positionCount; i++)
            {
                float resampledPhase = i / ((float) lineRenderer.positionCount-1);
                Vector3 resampledPos = Vector3.Lerp(startPosition, endPos, resampledPhase);
                resampledPos.x += (Mathf.PerlinNoise1D(resampledPos.y + breakHangNoiseOffset) - 0.5f) * phase * 2.8f;
                lineRenderer.SetPosition(i, resampledPos);
            }

            localTime += Time.deltaTime;
            return isFinished;
        }
    }
}
