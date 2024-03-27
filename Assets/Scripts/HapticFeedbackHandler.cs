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
        Lamp.OnLampDamaged += PerformDamageVibration;
        EnemyManager.OnFireflyExplosion += PerformExplosionVibration;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnPlayerAttack -= PerformTouchHaptic;
        Lamp.OnLampDamaged -= PerformDamageVibration;
        EnemyManager.OnFireflyExplosion -= PerformExplosionVibration;
    }

    private void PerformTouchHaptic()
    {
#if UNITY_ANDROID
        HapticFeedback.HeavyFeedback();
#endif
    }
    
    private void PerformDamageVibration(EnemyBase enemy)
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
    
    private void PerformExplosionVibration()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}
