using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsMeter : MonoBehaviour
{
    [SerializeField] private TMP_Text _fpsText;

    // Update is called once per frame
    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        _fpsText.text = $"FPS: {Mathf.Round(fps)}";
    }
}
