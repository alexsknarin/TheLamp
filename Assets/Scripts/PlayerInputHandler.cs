using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public static event Action OnPlayerAttack;
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            OnPlayerAttack?.Invoke();
        }
    }
}
