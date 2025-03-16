using System.Collections.Generic;
using UnityEngine;

namespace _GAME.Scripts.UI.UiElements
{
    public class UiUpgradePoints : MonoBehaviour
    {
        [SerializeField] private GameObject _upgradePointImagePrefab;
        private List<GameObject> _upgradePointImages = new List<GameObject>();
        private float _pointsMargin = 40f;
        private float _lastPointPosition = 0f;
    
        public void ShowUpgradePoints(int points)
        {
            if (points == 0)
            {
                foreach (var point in _upgradePointImages)
                {
                    point.SetActive(false);
                }
                return;
            }
        
            if(_upgradePointImages.Count < points)
            {
                int pointsToAdd = points - _upgradePointImages.Count;
                for (int i = 0; i < pointsToAdd; i++)
                {
                    var point = Instantiate(_upgradePointImagePrefab, transform);
                    point.transform.localPosition = Vector3.up * _lastPointPosition;
                    _lastPointPosition += _pointsMargin;
                    _upgradePointImages.Add(point);
                }
            }

            foreach (var point in _upgradePointImages)
            {
                point.SetActive(false);
            }
        
            for (int i=0; i<points; i++)
            {
                _upgradePointImages[i].SetActive(true);
            }
        
        
        }
    }
}
