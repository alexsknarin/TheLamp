using _GAME.Scripts.UI.ViewModels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _GAME.Scripts.UI.Views
{
    public class PlayerAttackUIView : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Button _button;
        private PlayerAttackViewModel _playerAttackViewModel;
    
        public void Bind(PlayerAttackViewModel playerAttackViewModel)
        {
            _playerAttackViewModel = playerAttackViewModel;
        }
    
        public void OnPointerDown(PointerEventData eventData)
        {
            _playerAttackViewModel.HandleAttackButtonClicked();
        }
    }
}
