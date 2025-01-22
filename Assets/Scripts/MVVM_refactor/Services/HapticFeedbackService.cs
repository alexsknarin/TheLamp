using System.Collections;
using UnityEngine;
#if UNITY_ANDROID
using CandyCoded.HapticFeedback;
#endif

// TODO: use interface and a strategy pattern to implement different platforms via DI

public class HapticFeedbackService
{
    private WaitForSeconds _vibrationDuration = new(0.2f); // TODO: move to settings magic number
    private bool _isDamageVibrationDisabled = true;
    
    private IEnumerator DisableHaptic()
    {
        yield return _vibrationDuration;
        _isDamageVibrationDisabled = true;
    }
    
    public void DoTouchHaptic()
    {
        Debug.Log("~~touch");
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_isDamageVibrationDisabled)
        {
            HapticFeedback.HeavyFeedback();    
        }
#endif
    }
    
    public  void DoDamageVibration()
    {
        Debug.Log("~~~~~~~~damage");
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
        _isDamageVibrationDisabled = false;
        StartCoroutine(DisableHaptic());
#endif
    }
    
    public void DoExplosionVibration()
    {
        Debug.Log("~~~~~~~~~~~explosion");
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
        _isDamageVibrationDisabled = false;
        StartCoroutine(DisableHaptic());
#endif
    }
}
