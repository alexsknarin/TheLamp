using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _GAME.Scripts.UI.UiElements
{
    [RequireComponent(typeof(Button))]
    public class FadableButtonPresentation : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Color _enabledTextColor = Color.white;
        [SerializeField] private Color _disabledTextColor = Color.grey;

        public void SetEnabled(bool isEnabled)
        {
            _button.interactable = isEnabled;
            _text.color = isEnabled ? _enabledTextColor : _disabledTextColor;
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
}
