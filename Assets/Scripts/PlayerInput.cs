using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static event Action OnPlayerAttack;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnPlayerAttack?.Invoke();
            Debug.Log("Player Attack");
        }
    }
}
