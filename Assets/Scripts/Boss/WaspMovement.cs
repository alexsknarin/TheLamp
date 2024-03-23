using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class WaspMovement : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private WaspStates _currentWaspState;
    private WaspStates _prevWaspState;
    [SerializeField] private Transform _baseTransform;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private WaspPlayablesContainer _playablesContainer;

    // Enter
    [SerializeField] private AnimationClip _waspEnterL;
    
    // Attack 1
    [SerializeField] private AnimationClip _waspAttackL1;
    [SerializeField] private AnimationClip _waspAttackL_Bounce;
    [SerializeField] private AnimationClip _waspAttackL1_Fail1;
    [SerializeField] private AnimationClip _waspAttackL1_Success1;
    [SerializeField] private AnimationClip _waspAttackL1_Success2;
    [SerializeField] private AnimationClip _waspAttackL1_Success3;
    
    // Attack 2
    [SerializeField] private AnimationClip _waspAttackL2;
    [SerializeField] private AnimationClip _waspAttackL2_Bounce;
    [SerializeField] private AnimationClip _waspAttackL2_Fail1;
    [SerializeField] private AnimationClip _waspAttackL2_Fail2;
    [SerializeField] private AnimationClip _waspAttackL2_Success1;
    
    // Attack 3
    [SerializeField] private AnimationClip _waspAttackL3;
    [SerializeField] private AnimationClip _waspAttackL3_Bounce;
    [SerializeField] private AnimationClip _waspAttackL3_Fail1;
    [SerializeField] private AnimationClip _waspAttackL3_Success1;
    
    // Attack 4
    [SerializeField] private AnimationClip _waspAttackL4;
    [SerializeField] private AnimationClip _waspAttackL4_Bounce;
    [SerializeField] private AnimationClip _waspAttackL4_Fail1;
    [SerializeField] private AnimationClip _waspAttackL4_Success1;
    
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    private int _attackVariant = 0;

    private void Start()
    {
        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        
        _playablesContainer = new WaspPlayablesContainer(_playableGraph);
        
        _playablesContainer.AddClip(WaspStates.EnterL, WaspStates.EnterR, _waspEnterL);
        _playablesContainer.AddClip(WaspStates.Attack1_L, WaspStates.Attack1_R, _waspAttackL1);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Bounce, WaspStates.Attack1_R_Bounce, _waspAttackL_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Fail1, WaspStates.Attack1_R_Fail1, _waspAttackL1_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Success1, WaspStates.Attack1_R_Success1, _waspAttackL1_Success1);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Success2, WaspStates.Attack1_R_Success2, _waspAttackL1_Success2);
        _playablesContainer.AddClip(WaspStates.Attack1_L_Success3, WaspStates.Attack1_R_Success3, _waspAttackL1_Success3);
        _playablesContainer.AddClip(WaspStates.Attack2_L, WaspStates.Attack2_R, _waspAttackL2);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Bounce, WaspStates.Attack2_R_Bounce, _waspAttackL2_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Fail1, WaspStates.Attack2_R_Fail1, _waspAttackL2_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Fail2, WaspStates.Attack2_R_Fail2, _waspAttackL2_Fail2);
        _playablesContainer.AddClip(WaspStates.Attack2_L_Success1, WaspStates.Attack2_R_Success1, _waspAttackL2_Success1);
        _playablesContainer.AddClip(WaspStates.Attack3_L, WaspStates.Attack3_R, _waspAttackL3);
        _playablesContainer.AddClip(WaspStates.Attack3_L_Bounce, WaspStates.Attack3_R_Bounce, _waspAttackL3_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack3_L_Fail1, WaspStates.Attack3_R_Fail1, _waspAttackL3_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack3_L_Success1, WaspStates.Attack3_R_Success1, _waspAttackL3_Success1);
        _playablesContainer.AddClip(WaspStates.Attack4_L, WaspStates.Attack4_R, _waspAttackL4);
        _playablesContainer.AddClip(WaspStates.Attack4_L_Bounce, WaspStates.Attack4_R_Bounce, _waspAttackL4_Bounce);
        _playablesContainer.AddClip(WaspStates.Attack4_L_Fail1, WaspStates.Attack4_R_Fail1, _waspAttackL4_Fail1);
        _playablesContainer.AddClip(WaspStates.Attack4_L_Success1, WaspStates.Attack4_R_Success1, _waspAttackL4_Success1);
        
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
                _currentWaspState = WaspStates.Attack1_L_Bounce;
                break;
            case WaspStates.Attack1_L_Bounce:
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
                _currentWaspState = WaspStates.Attack2_L_Bounce;
                break;
            case WaspStates.Attack2_L_Bounce:
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
                _currentWaspState = WaspStates.Attack3_L_Bounce;
                break;
            case WaspStates.Attack3_L_Bounce:
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
                _currentWaspState = WaspStates.Attack4_L_Bounce;
                break;
            case WaspStates.Attack4_L_Bounce:
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
                _currentWaspState = WaspStates.Attack1_R_Bounce;
                break;
            case WaspStates.Attack1_R_Bounce:
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
                _currentWaspState = WaspStates.Attack2_R_Bounce;
                break;
            case WaspStates.Attack2_R_Bounce:
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
                _currentWaspState = WaspStates.Attack3_R_Bounce;
                break;
            case WaspStates.Attack3_R_Bounce:
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
                _currentWaspState = WaspStates.Attack4_R_Bounce;
                break;
            case WaspStates.Attack4_R_Bounce:
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
