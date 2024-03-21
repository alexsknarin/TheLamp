using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public enum WaspStates
{
    EnterL, 
    Attack1_L,
    Attack1_L_Fail1,
    Attack1_L_Success1,
    Attack1_L_Success2,
    Attack1_L_Success3,
    Attack2_L,
    Attack2_L_Fail1,
    Attack2_L_Fail2,
    Attack2_L_Success1,
    Attack3_L,
    Attack3_L_Fail1,
    Attack3_L_Success1,
    Attack4_L,
    Attack4_L_Fail1,
    Attack4_L_Success1,
    EnterR,
    Attack1_R,
    Attack1_R_Fail1,
    Attack1_R_Success1,
    Attack1_R_Success2,
    Attack1_R_Success3,
    Attack2_R,
    Attack2_R_Fail1,
    Attack2_R_Fail2,
    Attack2_R_Success1,
    Attack3_R,
    Attack3_R_Fail1,
    Attack3_R_Success1,
    Attack4_R,
    Attack4_R_Fail1,
    Attack4_R_Success1
}


[RequireComponent(typeof(Animator))]
public class WaspMovement : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private WaspStates _currentWaspState;
    private WaspStates _prevWaspState;
    [SerializeField] private Transform _baseTransform;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    
    
    // Enter
    [SerializeField] private AnimationClip _waspEnterL;
    private AnimationClipPlayable _waspEnterLPlayable;
    
    // Attack 1
    [SerializeField] private AnimationClip _waspAttackL1;
    private AnimationClipPlayable _waspAttackL1Playable;
    
    [SerializeField] private AnimationClip _waspAttackL1_Fail1;
    private AnimationClipPlayable _waspAttackL1_Fail1Playable;
    
    [SerializeField] private AnimationClip _waspAttackL1_Success1;
    private AnimationClipPlayable _waspAttackL1_Success1Playable;
    
    [SerializeField] private AnimationClip _waspAttackL1_Success2;
    private AnimationClipPlayable _waspAttackL1_Success2Playable;    
    
    [SerializeField] private AnimationClip _waspAttackL1_Success3;
    private AnimationClipPlayable _waspAttackL1_Success3Playable;
    
    // Attack 2
    [SerializeField] private AnimationClip _waspAttackL2;
    private AnimationClipPlayable _waspAttackL2Playable;
    
    [SerializeField] private AnimationClip _waspAttackL2_Fail1;
    private AnimationClipPlayable _waspAttackL2_Fail1Playable;
    
    [SerializeField] private AnimationClip _waspAttackL2_Fail2;
    private AnimationClipPlayable _waspAttackL2_Fail2Playable;
    
    [SerializeField] private AnimationClip _waspAttackL2_Success1;
    private AnimationClipPlayable _waspAttackL2_Success1Playable;
    
    // Attack 3
    [SerializeField] private AnimationClip _waspAttackL3;
    private AnimationClipPlayable _waspAttackL3Playable;
    
    [SerializeField] private AnimationClip _waspAttackL3_Fail1;
    private AnimationClipPlayable _waspAttackL3_Fail1Playable;
    
    [SerializeField] private AnimationClip _waspAttackL3_Success1;
    private AnimationClipPlayable _waspAttackL3_Success1Playable;
    
    // Attack 4
    [SerializeField] private AnimationClip _waspAttackL4;
    private AnimationClipPlayable _waspAttackL4Playable;
    
    [SerializeField] private AnimationClip _waspAttackL4_Fail1;
    private AnimationClipPlayable _waspAttackL4_Fail1Playable;
    
    [SerializeField] private AnimationClip _waspAttackL4_Success1;
    private AnimationClipPlayable _waspAttackL4_Success1Playable;
    
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    private int _attackVariant = 0;

    private void Start()
    {
        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        _waspEnterLPlayable = AnimationClipPlayable.Create(_playableGraph, _waspEnterL);
        _waspAttackL1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL1);
        _waspAttackL1_Fail1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL1_Fail1);
        _waspAttackL1_Success1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL1_Success1);
        _waspAttackL1_Success2Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL1_Success2);
        _waspAttackL1_Success3Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL1_Success3);
        _waspAttackL2Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL2);
        _waspAttackL2_Fail1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL2_Fail1);
        _waspAttackL2_Fail2Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL2_Fail2);
        _waspAttackL2_Success1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL2_Success1);
        _waspAttackL3Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL3);
        _waspAttackL3_Fail1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL3_Fail1);
        _waspAttackL3_Success1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL3_Success1);
        _waspAttackL4Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL4);
        _waspAttackL4_Fail1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL4_Fail1);
        _waspAttackL4_Success1Playable = AnimationClipPlayable.Create(_playableGraph, _waspAttackL4_Success1);
        
        
        
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

    private AnimationClipPlayable GetAnimationStatePlayable(WaspStates state, ref int side)
    {
        switch (state)
        {
            case WaspStates.EnterL:
                side = 1;
                return _waspEnterLPlayable;
            case WaspStates.Attack1_L:
                side = 1;
                return _waspAttackL1Playable;
            case WaspStates.Attack1_L_Fail1:
                side = 1;
                return _waspAttackL1_Fail1Playable;
            case WaspStates.Attack1_L_Success1:
                side = 1;
                return _waspAttackL1_Success1Playable;
            case WaspStates.Attack1_L_Success2:
                side = 1;
                return _waspAttackL1_Success2Playable;
            case WaspStates.Attack1_L_Success3:
                side = 1;
                return _waspAttackL1_Success3Playable;
            case WaspStates.Attack2_L:
                side = 1;
                return _waspAttackL2Playable;
            case WaspStates.Attack2_L_Fail1:
                side = 1;
                return _waspAttackL2_Fail1Playable;
            case WaspStates.Attack2_L_Fail2:
                side = 1;
                return _waspAttackL2_Fail2Playable;
            case WaspStates.Attack2_L_Success1:
                side = 1;
                return _waspAttackL2_Success1Playable;
            case WaspStates.Attack3_L:
                side = 1;
                return _waspAttackL3Playable;
            case WaspStates.Attack3_L_Fail1:
                side = 1;
                return _waspAttackL3_Fail1Playable;
            case WaspStates.Attack3_L_Success1:
                side = 1;
                return _waspAttackL3_Success1Playable;
            case WaspStates.Attack4_L:
                side = 1;
                return _waspAttackL4Playable;
            case WaspStates.Attack4_L_Fail1:
                side = 1;
                return _waspAttackL4_Fail1Playable;
            case WaspStates.Attack4_L_Success1:
                side = 1;
                return _waspAttackL4_Success1Playable;
            case WaspStates.EnterR:
                side = -1;
                return _waspEnterLPlayable;
            case WaspStates.Attack1_R:
                side = -1;
                return _waspAttackL1Playable;
            case WaspStates.Attack1_R_Fail1:
                side = -1;
                return _waspAttackL1_Fail1Playable;
            case WaspStates.Attack1_R_Success1:
                side = -1;
                return _waspAttackL1_Success1Playable;
            case WaspStates.Attack1_R_Success2:
                side = -1;
                return _waspAttackL1_Success2Playable;
            case WaspStates.Attack1_R_Success3:
                side = -1;
                return _waspAttackL1_Success3Playable;
            case WaspStates.Attack2_R:
                side = -1;
                return _waspAttackL2Playable;
            case WaspStates.Attack2_R_Fail1:
                side = -1;
                return _waspAttackL2_Fail1Playable;
            case WaspStates.Attack2_R_Fail2:
                side = -1;
                return _waspAttackL2_Fail2Playable;
            case WaspStates.Attack2_R_Success1:
                side = -1;
                return _waspAttackL2_Success1Playable;
            case WaspStates.Attack3_R:
                side = -1;
                return _waspAttackL3Playable;
            case WaspStates.Attack3_R_Fail1:
                side = -1;
                return _waspAttackL3_Fail1Playable;
            case WaspStates.Attack3_R_Success1:
                side = -1;
                return _waspAttackL3_Success1Playable;
            case WaspStates.Attack4_R:
                side = -1;
                return _waspAttackL4Playable;
            case WaspStates.Attack4_R_Fail1:
                side = -1;
                return _waspAttackL4_Fail1Playable;
            case WaspStates.Attack4_R_Success1:
                side = -1;
                return _waspAttackL4_Success1Playable;
        }
        return _waspEnterLPlayable;
    }
    
    private void PlayStateClip(WaspStates state)
    {
        int side = 1;
        AnimationClipPlayable clipPlayable = GetAnimationStatePlayable(state, ref side);
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
                }
                break;
            // Attack1_L
            case WaspStates.Attack1_L:
                v = Random.Range(0, 4);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack1_L_Success1;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack1_L_Success2;
                }
                else if(v == 3)
                {
                    _currentWaspState = WaspStates.Attack1_L_Success3;
                }
                break;
            case WaspStates.Attack1_L_Fail1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_R;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack4_L;
                }
                break;
            case WaspStates.Attack1_L_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack2_R;
                }
                break;
            case WaspStates.Attack1_L_Success2:
                _currentWaspState = WaspStates.Attack4_L;    
                break;
            case WaspStates.Attack1_L_Success3:
                _currentWaspState = WaspStates.Attack2_L;    
                break;
            // Attack2_L
            case WaspStates.Attack2_L:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_L_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack2_L_Fail2;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack2_L_Success1;
                }
                break;
            case WaspStates.Attack2_L_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_R;
                }
                break;
            case WaspStates.Attack2_L_Fail2:
                _currentWaspState = WaspStates.Attack2_R;    
                break;
            case WaspStates.Attack2_L_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_L;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack4_L;
                }
                break;
            // Attack3_L
            case WaspStates.Attack3_L:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack3_L_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_L_Success1;
                }
                break;
            case WaspStates.Attack3_L_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                }
                break;
            case WaspStates.Attack3_L_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_L;
                }
                break;
            // Attack4_L
            case WaspStates.Attack4_L:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack4_L_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack4_L_Success1;
                }
                break;
            case WaspStates.Attack4_L_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                }
                break;
            case WaspStates.Attack4_L_Success1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack2_L;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                }
                break;

            // R
            case WaspStates.EnterR:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_R;
                }
                break;
            // Attack1_R
            case WaspStates.Attack1_R:
                v = Random.Range(0, 4);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack1_R_Success1;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack1_R_Success2;
                }
                else if(v == 3)
                {
                    _currentWaspState = WaspStates.Attack1_R_Success3;
                }
                break;
            case WaspStates.Attack1_R_Fail1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_L;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack4_R;
                }
                break;
            case WaspStates.Attack1_R_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;
                }
                else
                {
                    _currentWaspState = WaspStates.Attack2_L;
                }
                break;
            case WaspStates.Attack1_R_Success2:
                _currentWaspState = WaspStates.Attack4_R;    
                break;
            case WaspStates.Attack1_R_Success3:
                _currentWaspState = WaspStates.Attack2_R;    
                break;
            // Attack2_R
            case WaspStates.Attack2_R:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_R_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack2_R_Fail2;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack2_R_Success1;
                }
                break;
            case WaspStates.Attack2_R_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_L;
                }
                break;
            case WaspStates.Attack2_R_Fail2:
                _currentWaspState = WaspStates.Attack2_L;    
                break;
            case WaspStates.Attack2_R_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack2_R;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack4_R;
                }
                break;
            // Attack3_R
            case WaspStates.Attack3_R:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack3_R_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_R_Success1;
                }
                break;
            case WaspStates.Attack3_R_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_L;
                }
                break;
            case WaspStates.Attack3_R_Success1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;    
                }
                else
                {
                    _currentWaspState = WaspStates.Attack3_R;
                }
                break;
            // Attack4_R
            case WaspStates.Attack4_R:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack4_R_Fail1;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack4_R_Success1;
                }
                break;
            case WaspStates.Attack4_R_Fail1:
                v = Random.Range(0, 2);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_R;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                }
                break;
            case WaspStates.Attack4_R_Success1:
                v = Random.Range(0, 3);
                if (v == 0)
                {
                    _currentWaspState = WaspStates.Attack1_L;    
                }
                else if(v == 1)
                {
                    _currentWaspState = WaspStates.Attack2_R;
                }
                else if(v == 2)
                {
                    _currentWaspState = WaspStates.Attack3_R;
                }    
                break;
        }
        
        PlayStateClip(_currentWaspState);
    }
    
    private void OnDisable()
    {
        _playableGraph.Destroy();
    }
}
