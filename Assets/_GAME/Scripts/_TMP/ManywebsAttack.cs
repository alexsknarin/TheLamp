using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
// TODO: guided random - to avoid having wires close to each other
// TODO: find exit point
// TODO: what to do if all wire are damaged before Spider Collided
// TODO: dirty flag on wire to be sure that they won;t be reactivated by accident
// TODO: Normalize wire lengths ???
// TODO: collision in camera space
// TODO: add acceleration to spider attack


public class ManywebsAttack : MonoBehaviour
{
    private enum WireStates
    {
        Inactive,
        WireAttack,
        PreAttackPause,
        MainAttack
    }
    
    [SerializeField] private SpiderwebSpawnRange[] _webStartPositionsRanges;
    [SerializeField] private Transform _lampTransform;
    [SerializeField] private Vector3 _lampEndPoint;
    [SerializeField] private int _numberOfWires;
    [SerializeField] private float _attackTimeInterval;
    [SerializeField] private float _spiderAttackDelay;
    [SerializeField] private Vector3 _inactivePosition;
    [SerializeField] private WireStates _wireState = WireStates.Inactive;
    [SerializeField] private float _mainAttackDuration;
    [SerializeField] private float _mainAttackAcceleration;
    private SpiderwebAttackWire _mainAttackWire;
    
    private Vector3 _currentPosition;
    
    

    private bool _isWireAttacking = false;
    private bool _isSpiderAttacking = false;
    private int _currentAttackingWireIndex = 0;
    
    private float _localTime;

    private List<SpiderwebAttackWire> _attackWires = new();
    private List<int> _activeWireIndices = new();

    private void Awake()
    {
        _wireState = WireStates.Inactive;
        _currentPosition = _inactivePosition;
        CreateEmptyAttackWireVariables();
    }

    private void Start()
    {
        // Show source ranges
        foreach (var range in _webStartPositionsRanges)
        {
            Debug.DrawLine(range.p1, range.p2, Color.red, 10);
        }
        InitializeAttackWires();
        StartWireAttack();
    }
    
    private void StartWireAttack()
    {
        _currentAttackingWireIndex = 0;
        _localTime = 0f;
        _wireState = WireStates.WireAttack;
    }

    private void HandleWiresAttack()
    {
        float attackPhase = _localTime / _attackTimeInterval;
        if (attackPhase > 1)
        {
            _attackWires[_currentAttackingWireIndex].SetActive(true);
            _currentAttackingWireIndex++;
            _localTime = 0f;
            
            if (_currentAttackingWireIndex >= _numberOfWires)
            {
                StartPreAttackPause();
            }
        }
        _localTime += Time.deltaTime;
    }

    private void StartPreAttackPause()
    {
        _wireState = WireStates.PreAttackPause;
        _localTime = 0f;
    }

    private void HandlePreAttackPause()
    {
        if (_localTime / _spiderAttackDelay > 1)
        {
            StartMainAttack();            
        }
        _localTime += Time.deltaTime;
    }

    private void StartMainAttack()
    {
        _localTime = 0;
        _wireState = WireStates.MainAttack;
        RefreshActiveWiresList();
        _mainAttackWire = _attackWires[_activeWireIndices[Random.Range(0, _activeWireIndices.Count)]];
        Debug.DrawLine(_mainAttackWire.StartPosition, _mainAttackWire.EndPosition, Color.orangeRed, 10);
    }

    private void HandleMainAttack()
    {
        Debug.Log("Main Attack:");
        float phase = _localTime / _mainAttackDuration;
        Debug.Log("phase : " + phase);
        
        _currentPosition = Vector3.Lerp(
            _mainAttackWire.StartPosition, 
            _mainAttackWire.EndPosition, 
            Mathf.Pow(phase, _mainAttackAcceleration));
        
        Debug.Log("Current Position : " + _currentPosition);

        if ((_lampTransform.position - _currentPosition).magnitude < (0.5f + 0.325f))
        {
            Debug.Break();
        }
        _localTime += Time.deltaTime;
    }
    

    private void Update()
    {
        if (_wireState == WireStates.WireAttack)
        {
            HandleWiresAttack();
        }
        else if (_wireState == WireStates.PreAttackPause)
        {
            HandlePreAttackPause();
        }
        else if (_wireState == WireStates.MainAttack)
        {
            HandleMainAttack();
        }

        
        // TODO: Extract into method
        if (Input.GetKeyDown(KeyCode.A))
        {
            ReceiveWireDamage(3);
        }

        foreach (var wire in _attackWires)
        {
            wire.UpdateEndPosition(_lampTransform);
        }
    }

    private void ReceiveWireDamage(int power)
    {
        RefreshActiveWiresList();

        _attackWires[_activeWireIndices[Random.Range(0, _activeWireIndices.Count)]].ReceiveDamage(power);
    }

    private void RefreshActiveWiresList()
    {
        _activeWireIndices.Clear();

        for (int i = 0; i < _attackWires.Count; i++)
        {
            if (_attackWires[i].IsActive)
            {
                _activeWireIndices.Add(i);
            }
        }
    }


    private void CreateEmptyAttackWireVariables()
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_currentPosition, 0.325f);
    }
}
