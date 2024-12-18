using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackUIView : MonoBehaviour
{
    [SerializeField] private Button _button;
    private PlayerAttackViewModel _playerAttackViewModel;
    
    public void Construct(PlayerAttackViewModel playerAttackViewModel)
    {
        _playerAttackViewModel = playerAttackViewModel;
        _button.onClick.AddListener(_playerAttackViewModel.HandleAttackButtonClicked);
    }
    
    private void OnDestroy()
    {
        _button.onClick.RemoveListener(_playerAttackViewModel.HandleAttackButtonClicked);
    }
    
}
