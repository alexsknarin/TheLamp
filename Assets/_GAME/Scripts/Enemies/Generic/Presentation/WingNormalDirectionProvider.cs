using System;
using UnityEngine;

public class WingNormalDirectionProvider : MonoBehaviour
{
    private static readonly int BodyNormal = Shader.PropertyToID("_BodyNormal");
    private static readonly int CustomNormal = Shader.PropertyToID("_CustomNormal");
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
        _wingMotionBlurMaterial.SetVector(BodyNormal, _wingTransform.up);

        if (_useCustomNormal)
        {
            _wingMotionBlurMaterial.SetVector(CustomNormal, _wingCustomNormalTransform.up);
        }
    }
}
