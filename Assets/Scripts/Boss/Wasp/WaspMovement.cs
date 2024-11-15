using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class WaspMovement : MonoBehaviour, IInitializable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private WaspState _currentWaspState;
    [SerializeField] private Transform _baseTransform;
    private WaspState _prevWaspState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private WaspPlayablesContainer _playablesContainer;
    
    public event Action OnBossAttackStartedEvent;
    public event Action OnDeathStateEndedEvent;
    public event Action OnLeftTheScreenEvent;

    // Idle
    [SerializeField] private AnimationClip _waspIdle;
    
    // Enter
    [SerializeField] private AnimationClip _waspEnterL;
    
    // Attack 1
    [SerializeField] private AnimationClip _waspAttackL1;
    [SerializeField] private AnimationClip _waspAttackL_Bounce;
    [SerializeField] private AnimationClip _waspAttackL1_Fail1;
    [SerializeField] private AnimationClip _waspAttackL1_Success1;
    [SerializeField] private AnimationClip _waspAttackL1_Success2;
    [SerializeField] private AnimationClip _waspAttackL1_Success3;
    [SerializeField] private AnimationClip _waspAttackL1_Death;
    
    
    // Attack 2
    [SerializeField] private AnimationClip _waspAttackL2;
    [SerializeField] private AnimationClip _waspAttackL2_Bounce;
    [SerializeField] private AnimationClip _waspAttackL2_Fail1;
    [SerializeField] private AnimationClip _waspAttackL2_Fail2;
    [SerializeField] private AnimationClip _waspAttackL2_Success1;
    [SerializeField] private AnimationClip _waspAttackL2_Death;
    
    // Attack 3
    [SerializeField] private AnimationClip _waspAttackL3;
    [SerializeField] private AnimationClip _waspAttackL3_Bounce;
    [SerializeField] private AnimationClip _waspAttackL3_Fail1;
    [SerializeField] private AnimationClip _waspAttackL3_Success1;
    [SerializeField] private AnimationClip _waspAttackL3_Death;
    
    // Attack 4
    [SerializeField] private AnimationClip _waspAttackL4;
    [SerializeField] private AnimationClip _waspAttackL4_Bounce;
    [SerializeField] private AnimationClip _waspAttackL4_Fail1;
    [SerializeField] private AnimationClip _waspAttackL4_Success1;
    [SerializeField] private AnimationClip _waspAttackL4_Death;
    
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    // State parameters
    private bool _isDamaged = false;
    private bool _isDead = false;
    private bool _isLampDestroyed = false;
    
    public void Initialize()
    {
        _isLampDestroyed = false;
        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        _playablesContainer = new WaspPlayablesContainer(_playableGraph);
        _playablesContainer.AddSingleStateClip(WaspState.Idle, _waspIdle);
        _playablesContainer.AddClip(WaspState.EnterL, WaspState.EnterR, _waspEnterL);
        _playablesContainer.AddClip(WaspState.Attack1_L, WaspState.Attack1_R, _waspAttackL1);
        _playablesContainer.AddClip(WaspState.Attack1_L_Bounce, WaspState.Attack1_R_Bounce, _waspAttackL_Bounce);
        _playablesContainer.AddClip(WaspState.Attack1_L_Fail1, WaspState.Attack1_R_Fail1, _waspAttackL1_Fail1);
        _playablesContainer.AddClip(WaspState.Attack1_L_Success1, WaspState.Attack1_R_Success1, _waspAttackL1_Success1);
        _playablesContainer.AddClip(WaspState.Attack1_L_Success2, WaspState.Attack1_R_Success2, _waspAttackL1_Success2);
        _playablesContainer.AddClip(WaspState.Attack1_L_Success3, WaspState.Attack1_R_Success3, _waspAttackL1_Success3);
        _playablesContainer.AddClip(WaspState.Attack1_L_Death, WaspState.Attack1_R_Death, _waspAttackL1_Death);
        _playablesContainer.AddClip(WaspState.Attack2_L, WaspState.Attack2_R, _waspAttackL2);
        _playablesContainer.AddClip(WaspState.Attack2_L_Bounce, WaspState.Attack2_R_Bounce, _waspAttackL2_Bounce);
        _playablesContainer.AddClip(WaspState.Attack2_L_Fail1, WaspState.Attack2_R_Fail1, _waspAttackL2_Fail1);
        _playablesContainer.AddClip(WaspState.Attack2_L_Fail2, WaspState.Attack2_R_Fail2, _waspAttackL2_Fail2);
        _playablesContainer.AddClip(WaspState.Attack2_L_Success1, WaspState.Attack2_R_Success1, _waspAttackL2_Success1);
        _playablesContainer.AddClip(WaspState.Attack2_L_Death, WaspState.Attack2_R_Death, _waspAttackL2_Death);
        _playablesContainer.AddClip(WaspState.Attack3_L, WaspState.Attack3_R, _waspAttackL3);
        _playablesContainer.AddClip(WaspState.Attack3_L_Bounce, WaspState.Attack3_R_Bounce, _waspAttackL3_Bounce);
        _playablesContainer.AddClip(WaspState.Attack3_L_Fail1, WaspState.Attack3_R_Fail1, _waspAttackL3_Fail1);
        _playablesContainer.AddClip(WaspState.Attack3_L_Success1, WaspState.Attack3_R_Success1, _waspAttackL3_Success1);
        _playablesContainer.AddClip(WaspState.Attack3_L_Death, WaspState.Attack3_R_Death, _waspAttackL3_Death);
        _playablesContainer.AddClip(WaspState.Attack4_L, WaspState.Attack4_R, _waspAttackL4);
        _playablesContainer.AddClip(WaspState.Attack4_L_Bounce, WaspState.Attack4_R_Bounce, _waspAttackL4_Bounce);
        _playablesContainer.AddClip(WaspState.Attack4_L_Fail1, WaspState.Attack4_R_Fail1, _waspAttackL4_Fail1);
        _playablesContainer.AddClip(WaspState.Attack4_L_Success1, WaspState.Attack4_R_Success1, _waspAttackL4_Success1);
        _playablesContainer.AddClip(WaspState.Attack4_L_Death, WaspState.Attack4_R_Death, _waspAttackL4_Death);
        
        MovementReset();
    }

    public void MovementReset()
    {
        _isLampDestroyed = false;
        _isDamaged = false;
        _currentWaspState = WaspState.Idle;
    }

    public void Play()
    {
        int side = Random.Range(0, 2);
        if (side == 0)
        {
            _currentWaspState = WaspState.EnterL;
        }
        else
        {
            _currentWaspState = WaspState.EnterR;
        }

        PlayStateClip(_currentWaspState);
    }
    
    private void PlayStateClip(WaspState state)
    {
        AnimationClipPlayable clipPlayable = _playablesContainer.GetClip(state); 
        int side = _playablesContainer.GetSideDirection(state);    
        Vector3 scale = _baseTransform.localScale;
        scale.x = side;
        _baseTransform.localScale = scale;
        clipPlayable.SetTime(0);
        clipPlayable.SetTime(0); // Unity Bug
        _playableOutput.SetSourcePlayable(clipPlayable);
        if (_playableGraph.IsValid())
        {
            _playableGraph.Play();    
        }
    }
    
    public void EnableCollider()
    {
        _collider.enabled = true;
    }
    
    public void DisableCollider()
    {
        _collider.enabled = false;
    }

    public void ClipEnded()
    {
        SwitchState();
    }

    public void SetDamaged()
    {
        _isDamaged = true;
    }
    
    public void SetDead()
    {
        _isDead = true;
    }
    
    public void SetLampDestroyed()
    {
        _isLampDestroyed = true;
    }
    
    private void SwitchState()
    {
        int v = 0;
        switch (_currentWaspState)
        {
            case WaspState.EnterL:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_L;    
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            // Attack1_L
            case WaspState.Attack1_L:
                _currentWaspState = WaspState.Attack1_L_Bounce;
                break;
            case WaspState.Attack1_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack1_L_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspState.Attack1_L_Fail1;
                }
                else
                {
                    v = Random.Range(0, 3);
                    if(v == 0)
                    {
                        _currentWaspState = WaspState.Attack1_L_Success1;
                    }
                    else if(v == 1)
                    {
                        _currentWaspState = WaspState.Attack1_L_Success2;
                    }
                    else if(v == 2)
                    {
                        _currentWaspState = WaspState.Attack1_L_Success3;
                    }    
                }
                break;
            case WaspState.Attack1_L_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack2_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack3_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspState.Attack4_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack1_L_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack2_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack1_L_Success2:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                _currentWaspState = WaspState.Attack4_L;
                OnBossAttackStartedEvent?.Invoke();
                break;
            case WaspState.Attack1_L_Success3:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                _currentWaspState = WaspState.Attack2_L;
                OnBossAttackStartedEvent?.Invoke();
                break;
            // Attack2_L
            case WaspState.Attack2_L:
                _currentWaspState = WaspState.Attack2_L_Bounce;
                break;
            case WaspState.Attack2_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack2_L_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    v = Random.Range(0, 2);
                    if(v == 0)
                    {
                        _currentWaspState = WaspState.Attack2_L_Fail1;
                    }
                    if(v == 1)
                    {
                        _currentWaspState = WaspState.Attack2_L_Fail2;
                    }
                }
                else
                {
                    _currentWaspState = WaspState.Attack2_L_Success1;
                }
                break;
            case WaspState.Attack2_L_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack2_L_Fail2:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                _currentWaspState = WaspState.Attack2_R;
                OnBossAttackStartedEvent?.Invoke();
                break;
            case WaspState.Attack2_L_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack2_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack4_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            // Attack3_L
            case WaspState.Attack3_L:
                _currentWaspState = WaspState.Attack3_L_Bounce;
                break;
            case WaspState.Attack3_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack3_L_Death;
                    break;
                }
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspState.Attack3_L_Fail1;
                    
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_L_Success1;
                }
                break;
            case WaspState.Attack3_L_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_R;  
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack3_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack3_L_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_L; 
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            // Attack4_L
            case WaspState.Attack4_L:
                _currentWaspState = WaspState.Attack4_L_Bounce;
                break;
            case WaspState.Attack4_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack4_L_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspState.Attack4_L_Fail1;
                }
                else
                {
                    _currentWaspState = WaspState.Attack4_L_Success1;
                }
                break;
            case WaspState.Attack4_L_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_L;    
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack3_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack4_L_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_R;  
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack2_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspState.Attack3_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;

            // R
            case WaspState.EnterR:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_R;   
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            // Attack1_R
            case WaspState.Attack1_R:
                _currentWaspState = WaspState.Attack1_R_Bounce;
                break;
            case WaspState.Attack1_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack1_R_Death;
                    break;
                }
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspState.Attack1_R_Fail1;
                }
                else
                {
                    v = Random.Range(0, 3);
                    if(v == 0)
                    {
                        _currentWaspState = WaspState.Attack1_R_Success1;
                    }
                    else if(v == 1)
                    {
                        _currentWaspState = WaspState.Attack1_R_Success2;
                    }
                    else if(v == 2)
                    {
                        _currentWaspState = WaspState.Attack1_R_Success3;
                    }
                }
                break;
            case WaspState.Attack1_R_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack2_L;   
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack3_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspState.Attack4_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack1_R_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack2_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack1_R_Success2:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                _currentWaspState = WaspState.Attack4_R;
                OnBossAttackStartedEvent?.Invoke();
                break;
            case WaspState.Attack1_R_Success3:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                _currentWaspState = WaspState.Attack2_R;
                OnBossAttackStartedEvent?.Invoke();
                break;
            // Attack2_R
            case WaspState.Attack2_R:
                _currentWaspState = WaspState.Attack2_R_Bounce;
                break;
            case WaspState.Attack2_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack2_R_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    v = Random.Range(0, 2);
                    if (v == 0)
                    {
                        _currentWaspState = WaspState.Attack2_R_Fail1;    
                    }
                    else if(v == 1)
                    {
                        _currentWaspState = WaspState.Attack2_R_Fail2;
                    }    
                }
                else
                {
                    _currentWaspState = WaspState.Attack2_R_Success1;
                }
                break;
            case WaspState.Attack2_R_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_L; 
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack2_R_Fail2:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                _currentWaspState = WaspState.Attack2_L;  
                OnBossAttackStartedEvent?.Invoke();
                break;
            case WaspState.Attack2_R_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack2_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack4_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            // Attack3_R
            case WaspState.Attack3_R:
                _currentWaspState = WaspState.Attack3_R_Bounce;
                break;
            case WaspState.Attack3_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack3_R_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspState.Attack3_R_Fail1;
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_R_Success1;
                }
                break;
            case WaspState.Attack3_R_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_L;    
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack3_L;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack3_R_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_R; 
                    OnBossAttackStartedEvent?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspState.Attack3_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            // Attack4_R
            case WaspState.Attack4_R:
                _currentWaspState = WaspState.Attack4_R_Bounce;
                break;
            case WaspState.Attack4_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspState.Attack4_R_Death;
                    break;
                }
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspState.Attack4_R_Fail1;
                }
                else
                {
                    _currentWaspState = WaspState.Attack4_R_Success1;
                }
                break;
            case WaspState.Attack4_R_Fail1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_R;   
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack3_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                break;
            case WaspState.Attack4_R_Success1:
                if (_isLampDestroyed)
                {
                    _currentWaspState = WaspState.Idle;
                    break;
                }
                OnLeftTheScreenEvent?.Invoke();
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspState.Attack1_L;   
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspState.Attack2_R;
                    OnBossAttackStartedEvent?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspState.Attack3_R;
                    OnBossAttackStartedEvent?.Invoke();
                }    
                break;
            case WaspState.Attack1_L_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break;
            case WaspState.Attack2_L_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break; 
            case WaspState.Attack3_L_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break; 
            case WaspState.Attack4_L_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break; 
            case WaspState.Attack1_R_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break;
            case WaspState.Attack2_R_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break; 
            case WaspState.Attack3_R_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break; 
            case WaspState.Attack4_R_Death:
                _currentWaspState = WaspState.Idle;
                OnDeathStateEndedEvent?.Invoke();
                break;
        }
        
        PlayStateClip(_currentWaspState);
    }
    
    private void OnDisable()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
    }

    private void OnDestroy()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
    }
}
