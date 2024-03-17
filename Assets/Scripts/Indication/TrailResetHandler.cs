using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailResetHandler : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    private WaitForSeconds _waitTime = new WaitForSeconds(.25f);
    
    private IEnumerator EnableTrail()
    {
        yield return _waitTime;
        _trailRenderer.emitting = true;
    }
    
    public void Reset()
    {
        _trailRenderer.emitting = false;
        _trailRenderer.Clear();
        StartCoroutine(EnableTrail());
    }
}
