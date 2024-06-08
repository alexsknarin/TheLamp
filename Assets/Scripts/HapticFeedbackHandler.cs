#if UNITY_ANDROID
using CandyCoded.HapticFeedback;
#endif

using System;
using System.Collections;
using UnityEngine;

public class HapticFeedbackHandler : MonoBehaviour
{
    private bool _isDamageVibrationDisabled = true;
    private WaitForSeconds _vibrationDuration = new WaitForSeconds(0.2f);
    
    private void OnEnable()
    {
        PlayerInputHandler.OnPlayerAttack += PerformTouchHaptic;
        Lamp.OnLampDamaged += PerformDamageVibration;
        Lamp.OnLampDead += PerformDamageVibration;
        EnemyManager.OnFireflyExplosion += PerformExplosionVibration;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnPlayerAttack -= PerformTouchHaptic;
        Lamp.OnLampDamaged -= PerformDamageVibration;
        Lamp.OnLampDead -= PerformDamageVibration;
        EnemyManager.OnFireflyExplosion -= PerformExplosionVibration;
    }

    private IEnumerator DisableHaptic()
    {
        yield return _vibrationDuration;
        _isDamageVibrationDisabled = true;
    }

    private void PerformTouchHaptic()
    {
#if UNITY_ANDROID
        if (_isDamageVibrationDisabled)
        {
            HapticFeedback.HeavyFeedback();    
        }
#endif
    }
    
    private void PerformDamageVibration(EnemyBase enemy)
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
        _isDamageVibrationDisabled = false;
        StartCoroutine(DisableHaptic());
#endif
    }
    
    private void PerformExplosionVibration()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
        _isDamageVibrationDisabled = false;
        StartCoroutine(DisableHaptic());
#endif
    }
}
