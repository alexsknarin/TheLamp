using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _text;

    public void EnableButton()
    {
        _button.interactable = true;
        _text.color = Color.white;
    }
    
    public void DisableButton()
    {
        _button.interactable = false;
        _text.color = Color.grey;
    }
}
