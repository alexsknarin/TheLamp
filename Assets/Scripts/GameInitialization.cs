using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitialization : MonoBehaviour
{
    [SerializeField] private Lamp _lamp;

    // Start is called before the first frame update
    void Start()
    {
        _lamp.Initialize();   
    }

}
