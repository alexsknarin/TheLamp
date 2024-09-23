using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dragonfly : MonoBehaviour
{
    [SerializeField] private DragonflyMovement _dragonflyMovement;
    [Header("Hover")]
    [SerializeField] private float _hoverWaitMin;
    [SerializeField] private float _hoverWaitMax;
    private float _hoverWait;

    private float _localTime = 0;
    private bool _isWaitingForHoverAttack = false;

    private void OnEnable()
    {
        _dragonflyMovement.OnReadyToAttackStateEntered += OnReadyToAttackStateEntered;
        _dragonflyMovement.OnFallAnimationEnded += OnFallAnimationEnded;
    }
    
    private void OnDisable()
    {
        _dragonflyMovement.OnReadyToAttackStateEntered -= OnReadyToAttackStateEntered;
        _dragonflyMovement.OnFallAnimationEnded -= OnFallAnimationEnded;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _isWaitingForHoverAttack = false;
    }

    public void Play()
    {
        _dragonflyMovement.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
        
        if (_isWaitingForHoverAttack)
        {
            _localTime += Time.deltaTime;
            if (_localTime >= _hoverWait)
            {
                StartAttack();
                _isWaitingForHoverAttack = false;
            }
        }
    }
    
    private void OnReadyToAttackStateEntered(DragonflyStates state)
    {
        if (state == DragonflyStates.Hover)
        {
            PrepareHoverAttack();
        }
    }
    
    private void OnFallAnimationEnded(DragonflyStates state)
    {
        // Randomly select Patrol hover or catch spider
        _dragonflyMovement.Play();
        
    }

    private void PrepareHoverAttack()
    {
        Debug.Log("PrepareHoverAttack");
        _localTime = 0;
        _isWaitingForHoverAttack = true;
        _hoverWait = Random.Range(_hoverWaitMin, _hoverWaitMax);
    }
    
    private void StartAttack()
    {
        Debug.Log("StartAttack");
        _dragonflyMovement.StartAttack();
    }
}
