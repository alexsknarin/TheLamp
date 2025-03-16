using UnityEngine;

public class AdvertisementStageAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverUi;
    
    public void Play()
    {
        _gameOverUi.SetActive(false);
    }
}
