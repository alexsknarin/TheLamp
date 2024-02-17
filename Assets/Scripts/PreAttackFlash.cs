using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlyMovement))]
public class PreAttackFlash : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] FlyMovement _flyMovement;
    private Material _material;

    private void Start()
    {
        _material = _meshRenderer.material;
    }
    private void OnEnable()
    {
        _flyMovement.OnPreAttackStart += PreAttackStart;
        _flyMovement.OnPreAttackEnd += PreAttackEnd;
    }

    private void OnDisable()
    {
        _flyMovement.OnPreAttackStart -= PreAttackStart;
        _flyMovement.OnPreAttackEnd -= PreAttackEnd;
    }



    private void PreAttackStart()
    {
        _material.SetFloat("_AttackSemaphor", 1f);        
    }
    
    private void PreAttackEnd()
    {
        _material.SetFloat("_AttackSemaphor", 0f);   
    }
}
