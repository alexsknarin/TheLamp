using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FadableButtonPresentation : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Color _enabledTextColor = Color.white;
    [SerializeField] private Color _disabledTextColor = Color.grey;

    public void EnableButton()
    {
        _button.interactable = true;
        _text.color = _enabledTextColor;
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
