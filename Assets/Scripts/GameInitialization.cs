using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitialization : MonoBehaviour
{
    [SerializeField] private LampAttackModel _lampAttackModel;

    // Start is called before the first frame update
    void Start()
    {
        _lampAttackModel.Init();        
    }

}
