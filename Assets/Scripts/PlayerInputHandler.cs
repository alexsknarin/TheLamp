using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour,IInitializable
{
    private bool _isAttackAllowed = false;
    public static event Action OnPlayerAttackEvent;
   
    private void Update()
    {
        if (_isAttackAllowed)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnPlayerAttackEvent?.Invoke();
            }    
        }
    }
    
    public void HandleAttackButtonPress()
    {
        if (_isAttackAllowed)
        {
            OnPlayerAttackEvent?.Invoke();
        }
    }

    public void Initialize()
    {
        _isAttackAllowed = false;
    }

    public void EnableAttackInput()
    {
        _isAttackAllowed = true;
    }
    
    public void DisableAttackInput()
    {
        _isAttackAllowed = false;
    }
    
}
