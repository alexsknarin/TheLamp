using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour,IInitializable
{
    private bool _isAttackAllowed = false;
    public static event Action OnPlayerAttack;
   
    private void Update()
    {
        if (_isAttackAllowed)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnPlayerAttack?.Invoke();
            }    
        }
    }
    
    public void HandleAttackButtonPress()
    {
        if (_isAttackAllowed)
        {
            OnPlayerAttack?.Invoke();
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
