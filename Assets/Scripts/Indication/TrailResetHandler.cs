using System.Collections;
using UnityEngine;

public class TrailResetHandler : MonoBehaviour, IInitializable
{
    [SerializeField] private TrailRenderer _trailRenderer;
    private WaitForSeconds _waitTime = new WaitForSeconds(.25f);
    
    private IEnumerator EnableTrail()
    {
        yield return _waitTime;
        _trailRenderer.gameObject.SetActive(true);
        _trailRenderer.emitting = true;
    }

    public void Initialize()
    {
        _trailRenderer.emitting = false;
        _trailRenderer.Clear();
        StartCoroutine(EnableTrail());
    }
}
