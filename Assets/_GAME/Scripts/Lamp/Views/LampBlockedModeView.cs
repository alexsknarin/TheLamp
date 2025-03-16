using System;
using UnityEngine;

public class LampBlockedModeView : MonoBehaviour
{
    [SerializeField] private LampEmissionController _lampEmissionController;
    
    private PlayerGameplayViewModel _playerGameplayViewModel;

    public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
    {
        _playerGameplayViewModel = playerGameplayViewModel;
        _playerGameplayViewModel.IsBlocked.Changed += OnIsBlockedChanged;
    }

    private void OnDestroy()
    {
        _lampEmissionController.IsBlockedMode = false;
        _playerGameplayViewModel.IsBlocked.Changed += OnIsBlockedChanged;
    }

    private void OnIsBlockedChanged(object sender, Observable<bool>.ChangedEventArgs e)
    {
        Debug.Log("LampBlockedModeView : OnIsBlockedChanged");
        _lampEmissionController.IsBlockedMode = e.NewValue;
    }
}
