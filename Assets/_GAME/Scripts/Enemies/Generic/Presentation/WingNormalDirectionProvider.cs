using System;
using UnityEngine;

public class WingNormalDirectionProvider : MonoBehaviour
{
    [SerializeField] private Transform _wingTransform;
    [SerializeField] private Transform _wingCustomNormalTransform;
    [SerializeField] private bool _useCustomNormal = false;
    
    private Material _wingMotionBlurMaterial;

    // TODO: Make Initialize!!!!
    private void Awake()
    {
        _wingMotionBlurMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        _wingMotionBlurMaterial.SetVector("_WingNormal", _wingTransform.up);

        if (_useCustomNormal)
        {
            _wingMotionBlurMaterial.SetVector("_CustomWorldNormal", _wingCustomNormalTransform.up);
        }
    }
}
