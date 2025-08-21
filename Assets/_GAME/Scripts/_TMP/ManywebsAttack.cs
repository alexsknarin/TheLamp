using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManywebsAttack : MonoBehaviour
{
    [SerializeField] private SpiderwebSpawnRange[] _webStartPositionsRanges;
    [SerializeField] private Transform _lampTransform;
    [SerializeField] private Vector3 _lampEndPoint;
    [SerializeField] private int _numberOfWires;
    [SerializeField] private float _attackTimeInterval;
    [SerializeField] private float _spiderAttackDelay;

    private bool _isAttacking = false;
    private int _currentAttackingWireIndex = 0;
    
    private float _localTime;

    private List<SpiderwebAttackWire> _attackWires = new();
    private List<int> _activeWireIndices = new();

    private void Awake()
    {
        CreateAttackWires();
    }

    private void Start()
    {
        foreach (var range in _webStartPositionsRanges)
        {
            Debug.DrawLine(range.p1, range.p2, Color.red, 10);
        }

        InitializeAttackWires();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && !_isAttacking)
        {
            _isAttacking = true;
            _currentAttackingWireIndex = 0;
            _localTime = 0f;
            Debug.Log("Start Attack!!!!");
        }

        if (_isAttacking)
        {
            HandleWiresAttacking();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _activeWireIndices.Clear();

            for (int i = 0; i < _attackWires.Count; i++)
            {
                if (_attackWires[i].IsActive)
                {
                    _activeWireIndices.Add(i);
                }
            }
            
            Debug.Log("Active wires: " + _activeWireIndices.Count);
            
            _attackWires[_activeWireIndices[Random.Range(0, _activeWireIndices.Count)]].ReceiveDamage(3);
        }

        foreach (var wire in _attackWires)
        {
            wire.UpdateEndPosition(_lampTransform);
        }
    }

    private Vector3 GetWireStickPoint(Vector3 startPosition, Vector3 lampEndPoint)
    {
        Vector3 centerPoint = Vector3.zero;
        Vector3 startPoint = startPosition;
        Vector3 endPoint = lampEndPoint;

        Vector3 endToStartDir = (startPoint - endPoint).normalized;
        Vector3 endToCenterDir = (centerPoint - endPoint).normalized;

        float endAngle = Vector3.Angle(endToStartDir, endToCenterDir);
        float intersectionAngle = Mathf.Asin((0.35f * Mathf.Sin(Mathf.Deg2Rad * endAngle)) / 0.5f) * Mathf.Rad2Deg;
        float centerAngle = 180 - endAngle - intersectionAngle;

        float intersectionDistance =
            (0.5f * Mathf.Sin(Mathf.Deg2Rad * centerAngle)) / Mathf.Sin(Mathf.Deg2Rad * endAngle);

        Vector3 intesectionPos = endPoint + endToStartDir * intersectionDistance;
        
        return intesectionPos;
    }

    private void CreateAttackWires()
    {
        for (int i = 0; i < _numberOfWires; i++)
        {
            SpiderwebAttackWire wire = new();
            _attackWires.Add(wire);
        }
    }

    private void InitializeAttackWires() 
    {
        for (int i = 0; i < _numberOfWires; i++)
        {
            int randomIndex = Random.Range(0, _webStartPositionsRanges.Length);
            
            Vector3 startPoint = Vector3.Lerp(
                _webStartPositionsRanges[randomIndex].p1,
                _webStartPositionsRanges[randomIndex].p2,
                Random.Range(0f, 1f));
            _attackWires[i].Initialize(
                startPoint, 
                GetWireStickPoint(startPoint, _lampEndPoint)
            );
        }
    }

    private void HandleWiresAttacking()
    {
        float attackPhase = _localTime / _attackTimeInterval;
        if (attackPhase > 1)
        {
            _attackWires[_currentAttackingWireIndex].SetActive(true);
            _currentAttackingWireIndex++;
            _localTime = 0f;
            if (_currentAttackingWireIndex >= _numberOfWires)
            {
                _isAttacking = false;
            }
        }
        _localTime += Time.deltaTime;
    }
}
