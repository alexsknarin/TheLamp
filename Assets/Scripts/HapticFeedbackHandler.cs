#if UNITY_ANDROID
using CandyCoded.HapticFeedback;
#endif

using System;
using UnityEngine;

public class HapticFeedbackHandler : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerInputHandler.OnPlayerAttack += PerformTouchHaptic;
        Lamp.OnLampDamaged += PerformVibration;
        EnemyManager.OnFireflyExplosion += PerformVibration;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnPlayerAttack -= PerformTouchHaptic;
        Lamp.OnLampDamaged -= PerformVibration;
        EnemyManager.OnFireflyExplosion -= PerformVibration;
    }

    private void PerformTouchHaptic()
    {
#if UNITY_ANDROID
        HapticFeedback.HeavyFeedback();
#endif
    }
    
    private void PerformVibration()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}
