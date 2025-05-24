using System;
using UnityEngine;

public class WingNormalDirectionProvider : MonoBehaviour
{
    [SerializeField] private Transform _wingTransform;
    private Material _wingMotionBlurMaterial;

    private void Awake()
    {
        _wingMotionBlurMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        _wingMotionBlurMaterial.SetVector("_WingNormal", _wingTransform.up);
    }
}
