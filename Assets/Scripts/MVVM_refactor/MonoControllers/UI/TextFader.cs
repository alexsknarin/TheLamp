using TMPro;
using UnityEngine;

public class TextFader : MonoBehaviour
{
    [SerializeField] private TMP_Text _uiText;
    private Color _visibleColor = new Color(1, 1, 1, 1);
    private Color _invisibleColor = new Color(1, 1, 1, 0);
    
    public void SetText(string text)
    {
        _uiText.text = text;
    }
    
    public void SetVisibilityLevel(float visibilityLevel)
    {
        _uiText.color = Color.Lerp(_invisibleColor, _visibleColor, visibilityLevel);
    }
}
