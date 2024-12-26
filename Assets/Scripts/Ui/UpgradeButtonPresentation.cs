using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(TMP_Text))]
public class UpgradeButtonPresentation : MonoBehaviour
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

    public void SetVisibilityLevel(float visibilityLevel)
    {
        Color textColor = _text.color;
        textColor.a = visibilityLevel;
        _text.color = textColor;
        
        Color buttonColor = _button.image.color;
        buttonColor.a = visibilityLevel;
        _button.image.color = buttonColor;
    }
}
