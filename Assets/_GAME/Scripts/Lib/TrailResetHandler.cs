using System.Collections;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Lib
{
    public class TrailResetHandler : MonoBehaviour, IInitializable
    {
        [SerializeField] private TrailRenderer _trailRenderer;
        private WaitForSeconds _waitTime = new WaitForSeconds(.25f);

        public void Initialize()
        {
            _trailRenderer.emitting = false;
            _trailRenderer.Clear();
            StartCoroutine(EnableTrail());
        }

        private IEnumerator EnableTrail()
        {
            yield return _waitTime;
            _trailRenderer.gameObject.SetActive(true);
            _trailRenderer.emitting = true;
        }

        private void OnDisable()
        {
            _trailRenderer.gameObject.SetActive(false);
        }
    }
}
