using System.Collections;
using _GAME.Scripts.GameCoreSystems.DI;
using UnityEngine;
#if UNITY_ANDROID
using CandyCoded.HapticFeedback;
#endif

// TODO: use interface and a strategy pattern to implement different platforms via DI

namespace _GAME.Scripts.InGamePresentation.Haptic
{
    public class HapticFeedbackService
    {
        private readonly CoroutineHost _coroutineHost;
        
        private WaitForSeconds _vibrationDuration = new(0.2f); // TODO: move to settings magic number
        private bool _isDamageVibrationDisabled = true;
        
        public HapticFeedbackService(CoroutineHost coroutineHost)
        {
            _coroutineHost = coroutineHost;
        }
    
        private IEnumerator DisableHaptic()
        {
            yield return _vibrationDuration;
            _isDamageVibrationDisabled = true;
        }
    
        public void DoTouchHaptic()
        {
            // Debug.Log("~~touch");
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_isDamageVibrationDisabled)
        {
            HapticFeedback.HeavyFeedback();    
        }
#endif
        }
    
        public  void DoDamageVibration()
        {
            // Debug.Log("~~~~~~~~damage");
#if UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
            _isDamageVibrationDisabled = false;
            _coroutineHost.StartCoroutine(DisableHaptic());
#endif
        }
    
        public void DoExplosionVibration()
        {
            // Debug.Log("~~~~~~~~~~~explosion");
#if UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
            _isDamageVibrationDisabled = false;
            _coroutineHost.StartCoroutine(DisableHaptic());
#endif
        }
    }
}
