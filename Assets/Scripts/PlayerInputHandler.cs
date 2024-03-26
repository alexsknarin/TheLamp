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
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                OnPlayerAttack?.Invoke();
            }    
        }
    }

    public void Initialize()
    {
        _isAttackAllowed = false;
    }

    public void AllowAttackInput()
    {
        _isAttackAllowed = true;
    }
    
    public void ExitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
