using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class WaspMovement : MonoBehaviour, IInitializable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private WaspStates _currentWaspState;
    private WaspStates _prevWaspState;
    [SerializeField] private Transform _baseTransform;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private WaspPlayablesContainer _playablesContainer;
    
    
    
    public event Action OnWaspAttackStarted;
    public event Action OnDeathStateEnded;

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
    
    private int _attackVariant = 0;
    
    // State parameters
    private bool _isDamaged = false;
    private bool _isDead = false;
    
    public void Initialize()
    {
        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        _playablesContainer = new WaspPlayablesContainer(_playableGraph);
        _playablesContainer.AddSingleStateClip(WaspStates.Idle, _waspIdle);
        _playablesContainer.AddClip(WaspStates.EnterL, WaspStates.EnterR, _waspEnterL);
        _playablesContainer.AddClip(WaspStates.Attack1_L, WaspStates.Attack1_R, _waspAttackL1);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Bounce, WaspStates.Attack1_R_Bounce, _waspAttackL_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Fail1, WaspStates.Attack1_R_Fail1, _waspAttackL1_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Success1, WaspStates.Attack1_R_Success1, _waspAttackL1_Success1);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Success2, WaspStates.Attack1_R_Success2, _waspAttackL1_Success2);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Success3, WaspStates.Attack1_R_Success3, _waspAttackL1_Success3);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Death, WaspStates.Attack1_R_Death, _waspAttackL1_Death);
        _playablesContainer.AddClip(WaspStates.Attack2_L, WaspStates.Attack2_R, _waspAttackL2);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Bounce, WaspStates.Attack2_R_Bounce, _waspAttackL2_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Fail1, WaspStates.Attack2_R_Fail1, _waspAttackL2_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Fail2, WaspStates.Attack2_R_Fail2, _waspAttackL2_Fail2);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Success1, WaspStates.Attack2_R_Success1, _waspAttackL2_Success1);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Death, WaspStates.Attack2_R_Death, _waspAttackL2_Death);
        _playablesContainer.AddClip(WaspStates.Attack3_L, WaspStates.Attack3_R, _waspAttackL3);
        _playablesContainer.AddClip(WaspStates.Attack3_L_Bounce, WaspStates.Attack3_R_Bounce, _waspAttackL3_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack3_L_Fail1, WaspStates.Attack3_R_Fail1, _waspAttackL3_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack3_L_Success1, WaspStates.Attack3_R_Success1, _waspAttackL3_Success1);
        _playablesContainer.AddClip(WaspStates.Attack3_L_Death, WaspStates.Attack3_R_Death, _waspAttackL3_Death);
        _playablesContainer.AddClip(WaspStates.Attack4_L, WaspStates.Attack4_R, _waspAttackL4);
        _playablesContainer.AddClip(WaspStates.Attack4_L_Bounce, WaspStates.Attack4_R_Bounce, _waspAttackL4_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack4_L_Fail1, WaspStates.Attack4_R_Fail1, _waspAttackL4_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack4_L_Success1, WaspStates.Attack4_R_Success1, _waspAttackL4_Success1);
        _playablesContainer.AddClip(WaspStates.Attack4_L_Death, WaspStates.Attack4_R_Death, _waspAttackL4_Death);
        
        MovementReset();
    }

    public void MovementReset()
    {
        _isDamaged = false;
    }

    public void Play()
    {
        int side = Random.Range(0, 2);
        if (side == 0)
        {
            _currentWaspState = WaspStates.EnterL;
        }
        else
        {
            _currentWaspState = WaspStates.EnterR;
        }

        PlayStateClip(_currentWaspState);
    }
    
    private void PlayStateClip(WaspStates state)
    {
        AnimationClipPlayable clipPlayable = _playablesContainer.GetClip(state); 
        int side = _playablesContainer.GetSideDirection(state);    
        Vector3 scale = _baseTransform.localScale;
        scale.x = side;
        _baseTransform.localScale = scale;
        clipPlayable.SetTime(0);
        clipPlayable.SetTime(0); // Unity Bug
        _playableOutput.SetSourcePlayable(clipPlayable);
        _playableGraph.Play();
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
    
    private void SwitchState()
    {
        int v = 0;
        switch (_currentWaspState)
        {
            case WaspStates.EnterL:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            // Attack1_L
            case WaspStates.Attack1_L:
                _currentWaspState = WaspStates.Attack1_L_Bounce;
                break;
            case WaspStates.Attack1_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack1_L_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspStates.Attack1_L_Fail1;
                }
                else
                {
                    v = Random.Range(0, 3);
                    if(v == 0)
                    {
                        _currentWaspState = WaspStates.Attack1_L_Success1;
                    }
                    else if(v == 1)
                    {
                        _currentWaspState = WaspStates.Attack1_L_Success2;
                    }
                    else if(v == 2)
                    {
                        _currentWaspState = WaspStates.Attack1_L_Success3;
                    }    
                }
                break;
            case WaspStates.Attack1_L_Fail1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_R;
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack4_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack1_L_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack2_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack1_L_Success2:
                _currentWaspState = WaspStates.Attack4_L;
                OnWaspAttackStarted?.Invoke();
                break;
            case WaspStates.Attack1_L_Success3:
                _currentWaspState = WaspStates.Attack2_L;
                OnWaspAttackStarted?.Invoke();
                break;
            // Attack2_L
            case WaspStates.Attack2_L:
                _currentWaspState = WaspStates.Attack2_L_Bounce;
                break;
            case WaspStates.Attack2_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack2_L_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    v = Random.Range(0, 2);
                    if(v == 0)
                    {
                        _currentWaspState = WaspStates.Attack2_L_Fail1;
                    }
                    if(v == 1)
                    {
                        _currentWaspState = WaspStates.Attack2_L_Fail2;
                    }
                }
                else
                {
                    _currentWaspState = WaspStates.Attack2_L_Success1;
                }
                break;
            case WaspStates.Attack2_L_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack2_L_Fail2:
                _currentWaspState = WaspStates.Attack2_R;
                OnWaspAttackStarted?.Invoke();
                break;
            case WaspStates.Attack2_L_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_L;
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack4_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            // Attack3_L
            case WaspStates.Attack3_L:
                _currentWaspState = WaspStates.Attack3_L_Bounce;
                break;
            case WaspStates.Attack3_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack3_L_Death;
                    break;
                }
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspStates.Attack3_L_Fail1;
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_L_Success1;
                }
                break;
            case WaspStates.Attack3_L_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;  
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack3_L_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L; 
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            // Attack4_L
            case WaspStates.Attack4_L:
                _currentWaspState = WaspStates.Attack4_L_Bounce;
                break;
            case WaspStates.Attack4_L_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack4_L_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspStates.Attack4_L_Fail1;
                }
                else
                {
                    _currentWaspState = WaspStates.Attack4_L_Success1;
                }
                break;
            case WaspStates.Attack4_L_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack4_L_Success1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;  
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack2_L;
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;

            // R
            case WaspStates.EnterR:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;   
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            // Attack1_R
            case WaspStates.Attack1_R:
                _currentWaspState = WaspStates.Attack1_R_Bounce;
                break;
            case WaspStates.Attack1_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack1_R_Death;
                    break;
                }
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspStates.Attack1_R_Fail1;
                }
                else
                {
                    v = Random.Range(0, 3);
                    if(v == 0)
                    {
                        _currentWaspState = WaspStates.Attack1_R_Success1;
                    }
                    else if(v == 1)
                    {
                        _currentWaspState = WaspStates.Attack1_R_Success2;
                    }
                    else if(v == 2)
                    {
                        _currentWaspState = WaspStates.Attack1_R_Success3;
                    }
                }
                break;
            case WaspStates.Attack1_R_Fail1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_L;   
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack4_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack1_R_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack2_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack1_R_Success2:
                _currentWaspState = WaspStates.Attack4_R;
                OnWaspAttackStarted?.Invoke();
                break;
            case WaspStates.Attack1_R_Success3:
                _currentWaspState = WaspStates.Attack2_R;
                OnWaspAttackStarted?.Invoke();
                break;
            // Attack2_R
            case WaspStates.Attack2_R:
                _currentWaspState = WaspStates.Attack2_R_Bounce;
                break;
            case WaspStates.Attack2_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack2_R_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    v = Random.Range(0, 2);
                    if (v == 0)
                    {
                        _currentWaspState = WaspStates.Attack2_R_Fail1;    
                    }
                    else if(v == 1)
                    {
                        _currentWaspState = WaspStates.Attack2_R_Fail2;
                    }    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack2_R_Success1;
                }
                break;
            case WaspStates.Attack2_R_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L; 
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack2_R_Fail2:
                _currentWaspState = WaspStates.Attack2_L;  
                OnWaspAttackStarted?.Invoke();
                break;
            case WaspStates.Attack2_R_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_R;
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack4_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            // Attack3_R
            case WaspStates.Attack3_R:
                _currentWaspState = WaspStates.Attack3_R_Bounce;
                break;
            case WaspStates.Attack3_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack3_R_Death;
                    break;
                }
                
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspStates.Attack3_R_Fail1;
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_R_Success1;
                }
                break;
            case WaspStates.Attack3_R_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack3_R_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R; 
                    OnWaspAttackStarted?.Invoke();
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            // Attack4_R
            case WaspStates.Attack4_R:
                _currentWaspState = WaspStates.Attack4_R_Bounce;
                break;
            case WaspStates.Attack4_R_Bounce:
                if (_isDead)
                {
                    _isDead = false;
                    _currentWaspState = WaspStates.Attack4_R_Death;
                    break;
                }
                if (_isDamaged)
                {
                    _isDamaged = false;
                    _currentWaspState = WaspStates.Attack4_R_Fail1;
                }
                else
                {
                    _currentWaspState = WaspStates.Attack4_R_Success1;
                }
                break;
            case WaspStates.Attack4_R_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;   
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                    OnWaspAttackStarted?.Invoke();
                }
                break;
            case WaspStates.Attack4_R_Success1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;   
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack2_R;
                    OnWaspAttackStarted?.Invoke();
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                    OnWaspAttackStarted?.Invoke();
                }    
                break;
            case WaspStates.Attack1_L_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break;
            case WaspStates.Attack2_L_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break; 
            case WaspStates.Attack3_L_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break; 
            case WaspStates.Attack4_L_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break; 
            case WaspStates.Attack1_R_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break;
            case WaspStates.Attack2_R_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break; 
            case WaspStates.Attack3_R_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break; 
            case WaspStates.Attack4_R_Death:
                _currentWaspState = WaspStates.Idle;
                OnDeathStateEnded?.Invoke();
                break;
        }
        
        PlayStateClip(_currentWaspState);
    }
    
    private void OnDisable()
    {
        _playableGraph.Destroy();
    }
}
