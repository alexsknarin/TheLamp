using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailResetHandler : MonoBehaviour, IInitializable
{
    [SerializeField] private TrailRenderer _trailRenderer;
    private WaitForSeconds _waitTime = new WaitForSeconds(.2f);
    
    private IEnumerator EnableTrail()
    {
        yield return _waitTime;
        _trailRenderer.emitting = true;
    }

    public void Initialize()
    {
        _trailRenderer.emitting = false;
        _trailRenderer.Clear();
        StartCoroutine(EnableTrail());
    }
}
