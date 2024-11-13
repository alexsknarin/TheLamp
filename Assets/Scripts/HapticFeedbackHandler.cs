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
        PlayerInputHandler.OnPlayerAttackEvent += PerformTouchHaptic;
        Lamp.OnLampDamagedEvent += PerformDamageVibration;
        Lamp.OnLampDeadEvent += PerformDamageVibration;
        EnemyManager.OnFireflyExplosionEvent += PerformExplosionVibration;
    }

    private void OnDisable()
    {
        PlayerInputHandler.OnPlayerAttackEvent -= PerformTouchHaptic;
        Lamp.OnLampDamagedEvent -= PerformDamageVibration;
        Lamp.OnLampDeadEvent -= PerformDamageVibration;
        EnemyManager.OnFireflyExplosionEvent -= PerformExplosionVibration;
    }

    private IEnumerator DisableHaptic()
    {
        yield return _vibrationDuration;
        _isDamageVibrationDisabled = true;
    }

    private void PerformTouchHaptic()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_isDamageVibrationDisabled)
        {
            HapticFeedback.HeavyFeedback();    
        }
#endif
    }
    
    private void PerformDamageVibration(EnemyBase enemy)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
        _isDamageVibrationDisabled = false;
        StartCoroutine(DisableHaptic());
#endif
    }
    
    private void PerformExplosionVibration()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
        _isDamageVibrationDisabled = false;
        StartCoroutine(DisableHaptic());
#endif
    }
}
