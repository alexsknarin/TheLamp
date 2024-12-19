using System;
using System.Collections;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private WaitForSeconds _waitDuraiton;
    public event Action OnAttackEndedEvent; 
    
    public void SetDuration(float duration)
    {
        _waitDuraiton = new WaitForSeconds(duration);
    }
    
    public void Play()
    {
        StartCoroutine(WaitForAttackEnd());
    }
    
    private IEnumerator WaitForAttackEnd()
    {
        yield return _waitDuraiton;
        OnAttackEndedEvent?.Invoke();
    }
    
    
}
