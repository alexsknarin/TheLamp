using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FWaspMovement : MonoBehaviour, IInitializable
{
    private readonly List<Type> TWO_OPTION_OUTCOME = new()
    {
        typeof(FWaspEnterLState),
        typeof(FWaspEnterRState),
        typeof(FWaspAttack01Success01LState),
        typeof(FWaspAttack01Success01RState),
        typeof(FWaspAttack02Success01LState),
        typeof(FWaspAttack02Success01RState),
        typeof(FWaspAttack03Success01LState),
        typeof(FWaspAttack03Success01RState),
        typeof(FWaspAttack02BounceLState),
        typeof(FWaspAttack02BounceRState),
        typeof(FWaspAttack02Fail01LState),
        typeof(FWaspAttack02Fail01RState),
        typeof(FWaspAttack03Fail01LState),
        typeof(FWaspAttack03Fail01RState),
        typeof(FWaspAttack04Fail01LState),
        typeof(FWaspAttack04Fail01RState)
    };

    private readonly List<Type> THREE_OPTION_OUTCOME = new()
    {
        typeof(FWaspAttack01BounceLState),
        typeof(FWaspAttack01BounceRState),
        typeof(FWaspAttack04Success01LState),
        typeof(FWaspAttack04Success01RState),
        typeof(FWaspAttack01Fail01LState)
    };

    private readonly List<Type> ATTACK_STATES = new()
    {
        typeof(FWaspAttack01LState),
        typeof(FWaspAttack01RState),
        typeof(FWaspAttack02LState),
        typeof(FWaspAttack02RState),
        typeof(FWaspAttack03LState),
        typeof(FWaspAttack03RState),
        typeof(FWaspAttack04LState),
        typeof(FWaspAttack04RState)        
    };
    
    [SerializeField] private string _currentStateType;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _baseTransform;
    private ILampPositionProviderService _lampPositionProvider;


    private FStateMachine _stateMachine = new();
    private float _colliderRadius = 0.24f;
    private bool _isPlaying = false;
    // State parameters
    private Side _side = Side.Left;
    private bool _isAnimClipEnded = false;
    private int _twoOptiosSplit = 0;
    private int _threeOptiosSplit = 0;
    private bool _isDamaged = false;
    private bool _isDead = false;
    private bool _isLampDestroyed = false; // TODO: replace all this with enum? sucess, damaged, dead, lampDestroyed
    private bool _isCollided = false;
    // Animation 
    private readonly int _idleHash = Animator.StringToHash("Idle");
    private readonly int _enterHash = Animator.StringToHash("Enter");
    private readonly int _attack01Hash = Animator.StringToHash("Attack01");
    private readonly int _attack01BounceHash = Animator.StringToHash("Attack01_Bounce");
    private readonly int _attack01Fail01Hash = Animator.StringToHash("Attack01_Fail01");
    private readonly int _attack01Success01Hash = Animator.StringToHash("Attack01_Success01");
    private readonly int _attack01Success02Hash = Animator.StringToHash("Attack01_Success02");
    private readonly int _attack01Success03Hash = Animator.StringToHash("Attack01_Success03");
    private readonly int _attack01Death01Hash = Animator.StringToHash("Attack01_Death01");
    private readonly int _attack02Hash = Animator.StringToHash("Attack02");
    private readonly int _attack02BounceHash = Animator.StringToHash("Attack02_Bounce");
    private readonly int _attack02Fail01Hash = Animator.StringToHash("Attack02_Fail01");
    private readonly int _attack02Fail02Hash = Animator.StringToHash("Attack02_Fail02");
    private readonly int _attack02Success01Hash = Animator.StringToHash("Attack02_Success01");
    private readonly int _attack02Death01Hash = Animator.StringToHash("Attack02_Death01");
    private readonly int _attack03Hash = Animator.StringToHash("Attack03");
    private readonly int _attack03BounceHash = Animator.StringToHash("Attack03_Bounce");
    private readonly int _attack03Fail01Hash = Animator.StringToHash("Attack03_Fail01");
    private readonly int _attack03Success01Hash = Animator.StringToHash("Attack03_Success01");
    private readonly int _attack03Death01Hash = Animator.StringToHash("Attack03_Death01");
    private readonly int _attack04Hash = Animator.StringToHash("Attack04");
    private readonly int _attack04BounceHash = Animator.StringToHash("Attack04_Bounce");
    private readonly int _attack04Fail01Hash = Animator.StringToHash("Attack04_Fail01");
    private readonly int _attack04Success01Hash = Animator.StringToHash("Attack04_Success01");
    private readonly int _attack04Death01Hash = Animator.StringToHash("Attack04_Death01");
    // States
    private FWaspIdleState _idleState;
    private FWaspEnterLState _enterLState;
    private FWaspEnterRState _enterRState;
    private FWaspAttack01LState _attack01LState;
    private FWaspAttack01RState _attack01RState;
    private FWaspAttack01BounceLState _attack01BounceLState;
    private FWaspAttack01BounceRState _attack01BounceRState;
    private FWaspAttack01Fail01LState _attack01Fail01LState;
    private FWaspAttack01Fail01RState _attack01Fail01RState;
    private FWaspAttack01Success01LState _attack01Success01LState;
    private FWaspAttack01Success01RState _attack01Success01RState;
    private FWaspAttack01Success02LState _attack01Success02LState;
    private FWaspAttack01Success02RState _attack01Success02RState;
    private FWaspAttack01Success03LState _attack01Success03LState;
    private FWaspAttack01Success03RState _attack01Success03RState;
    private FWaspAttack01DeathLState _attack01DeathLState;
    private FWaspAttack01DeathRState _attack01DeathRState;
    private FWaspAttack02LState _attack02LState;
    private FWaspAttack02RState _attack02RState;
    private FWaspAttack02BounceLState _attack02BounceLState;
    private FWaspAttack02BounceRState _attack02BounceRState;
    private FWaspAttack02Fail01LState _attack02Fail01LState;
    private FWaspAttack02Fail01RState _attack02Fail01RState;
    private FWaspAttack02Fail02LState _attack02Fail02LState;
    private FWaspAttack02Fail02RState _attack02Fail02RState;
    private FWaspAttack02Success01LState _attack02Success01LState;
    private FWaspAttack02Success01RState _attack02Success01RState;
    private FWaspAttack02DeathLState _attack02DeathLState;
    private FWaspAttack02DeathRState _attack02DeathRState;
    private FWaspAttack03LState _attack03LState;
    private FWaspAttack03RState _attack03RState;
    private FWaspAttack03BounceLState _attack03BounceLState;
    private FWaspAttack03BounceRState _attack03BounceRState;
    private FWaspAttack03Fail01LState _attack03Fail01LState;
    private FWaspAttack03Fail01RState _attack03Fail01RState;
    private FWaspAttack03Success01LState _attack03Success01LState;
    private FWaspAttack03Success01RState _attack03Success01RState;
    private FWaspAttack03DeathLState _attack03DeathLState;
    private FWaspAttack03DeathRState _attack03DeathRState;
    private FWaspAttack04LState _attack04LState;
    private FWaspAttack04RState _attack04RState;
    private FWaspAttack04BounceLState _attack04BounceLState;
    private FWaspAttack04BounceRState _attack04BounceRState;
    private FWaspAttack04Fail01LState _attack04Fail01LState;
    private FWaspAttack04Fail01RState _attack04Fail01RState;
    private FWaspAttack04Success01LState _attack04Success01LState;
    private FWaspAttack04Success01RState _attack04Success01RState;
    private FWaspAttack04DeathLState _attack04DeathLState;
    private FWaspAttack04DeathRState _attack04DeathRState;

    public void Construct(ILampPositionProviderService lampPositionProvider)
    {
        _lampPositionProvider = lampPositionProvider;
    }

    public event Action BossAttackStarted;
    public event Action DeathStateEnded;


    private void OnEnable()
    {
        _attack01LState.Started += OnBossAttackStarted;
        _attack01RState.Started += OnBossAttackStarted;
        _attack02LState.Started += OnBossAttackStarted;
        _attack02RState.Started += OnBossAttackStarted;
        _attack03LState.Started += OnBossAttackStarted;
        _attack03RState.Started += OnBossAttackStarted;
        _attack04LState.Started += OnBossAttackStarted;
        _attack04RState.Started += OnBossAttackStarted;
        
        _attack01DeathLState.Ended += OnDeathStateEnded;
        _attack01DeathRState.Ended += OnDeathStateEnded;
        _attack02DeathLState.Ended += OnDeathStateEnded;
        _attack02DeathRState.Ended += OnDeathStateEnded;
        _attack03DeathLState.Ended += OnDeathStateEnded;
        _attack03DeathRState.Ended += OnDeathStateEnded;
        _attack04DeathLState.Ended += OnDeathStateEnded;
        _attack04DeathRState.Ended += OnDeathStateEnded;
    }

    private void OnDisable()
    {
        _attack01LState.Started -= OnBossAttackStarted;
        _attack01RState.Started -= OnBossAttackStarted;
        _attack02LState.Started -= OnBossAttackStarted;
        _attack02RState.Started -= OnBossAttackStarted;
        _attack03LState.Started -= OnBossAttackStarted;
        _attack03RState.Started -= OnBossAttackStarted;
        _attack04LState.Started -= OnBossAttackStarted;
        _attack04RState.Started -= OnBossAttackStarted;
        
        _attack01DeathLState.Ended -= OnDeathStateEnded;
        _attack01DeathRState.Ended -= OnDeathStateEnded;
        _attack02DeathLState.Ended -= OnDeathStateEnded;
        _attack02DeathRState.Ended -= OnDeathStateEnded;
        _attack03DeathLState.Ended -= OnDeathStateEnded;
        _attack03DeathRState.Ended -= OnDeathStateEnded;
        _attack04DeathLState.Ended -= OnDeathStateEnded;
        _attack04DeathRState.Ended -= OnDeathStateEnded;
    }


    public void Initialize()
    {
        CreateMovementStates();
        StateMachineSetup();
        
        _isPlaying = false;
        _isAnimClipEnded = false;
        _isLampDestroyed = false;
        _isDamaged = false;
        _isCollided = false;
    }

    public void Play()
    {
        _isPlaying = true;
        _isAnimClipEnded = false;
        _side = (Side)Random.Range(0, 2);
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


    public void ClipEnded()
    {
        _isAnimClipEnded = true;

        if (_isDead || _isLampDestroyed)
        {
            return;
        }
        
        if (TWO_OPTION_OUTCOME.Contains(_stateMachine.CurrentStateType))
        {
            _twoOptiosSplit = Random.Range(0, 2);
        }
        
        if (THREE_OPTION_OUTCOME.Contains(_stateMachine.CurrentStateType))
        {
            _threeOptiosSplit = Random.Range(0, 3);
        }
    }

    public void SetCollidedWithLamp()
    {
        _isCollided = true;
    }

    private void CreateMovementStates()
    {
        _idleState = new FWaspIdleState(_animator, _idleHash, _baseTransform);
        _enterLState = new FWaspEnterLState(_animator, _enterHash, _baseTransform);
        _enterRState = new FWaspEnterRState(_animator, _enterHash, _baseTransform);
        _attack01LState = new FWaspAttack01LState(_animator, _attack01Hash, _baseTransform);
        _attack01RState = new FWaspAttack01RState(_animator, _attack01Hash, _baseTransform);
        _attack01BounceLState = new FWaspAttack01BounceLState(_animator, _attack01BounceHash, _baseTransform);
        _attack01BounceRState = new FWaspAttack01BounceRState(_animator, _attack01BounceHash, _baseTransform);
        _attack01Fail01LState = new FWaspAttack01Fail01LState(_animator, _attack01Fail01Hash, _baseTransform);
        _attack01Fail01RState = new FWaspAttack01Fail01RState(_animator, _attack01Fail01Hash, _baseTransform);
        _attack01Success01LState = new FWaspAttack01Success01LState(_animator, _attack01Success01Hash, _baseTransform);
        _attack01Success01RState = new FWaspAttack01Success01RState(_animator, _attack01Success01Hash, _baseTransform);
        _attack01Success02LState = new FWaspAttack01Success02LState(_animator, _attack01Success02Hash, _baseTransform);
        _attack01Success02RState = new FWaspAttack01Success02RState(_animator, _attack01Success02Hash, _baseTransform);
        _attack01Success03LState = new FWaspAttack01Success03LState(_animator, _attack01Success03Hash, _baseTransform);
        _attack01Success03RState = new FWaspAttack01Success03RState(_animator, _attack01Success03Hash, _baseTransform);
        _attack01DeathLState = new FWaspAttack01DeathLState(_animator, _attack01Death01Hash, _baseTransform);
        _attack01DeathRState = new FWaspAttack01DeathRState(_animator, _attack01Death01Hash, _baseTransform);
        _attack02LState = new FWaspAttack02LState(_animator, _attack02Hash, _baseTransform);
        _attack02RState = new FWaspAttack02RState(_animator, _attack02Hash, _baseTransform);
        _attack02BounceLState = new FWaspAttack02BounceLState(_animator, _attack02BounceHash, _baseTransform);
        _attack02BounceRState = new FWaspAttack02BounceRState(_animator, _attack02BounceHash, _baseTransform);
        _attack02Fail01LState = new FWaspAttack02Fail01LState(_animator, _attack02Fail01Hash, _baseTransform);
        _attack02Fail01RState = new FWaspAttack02Fail01RState(_animator, _attack02Fail01Hash, _baseTransform);
        _attack02Fail02LState = new FWaspAttack02Fail02LState(_animator, _attack02Fail02Hash, _baseTransform);
        _attack02Fail02RState = new FWaspAttack02Fail02RState(_animator, _attack02Fail02Hash, _baseTransform);
        _attack02Success01LState = new FWaspAttack02Success01LState(_animator, _attack02Success01Hash, _baseTransform);
        _attack02Success01RState = new FWaspAttack02Success01RState(_animator, _attack02Success01Hash, _baseTransform);
        _attack02DeathLState = new FWaspAttack02DeathLState(_animator, _attack02Death01Hash, _baseTransform);
        _attack02DeathRState = new FWaspAttack02DeathRState(_animator, _attack02Death01Hash, _baseTransform);
        _attack03LState = new FWaspAttack03LState(_animator, _attack03Hash, _baseTransform);
        _attack03RState = new FWaspAttack03RState(_animator, _attack03Hash, _baseTransform);
        _attack03BounceLState = new FWaspAttack03BounceLState(_animator, _attack03BounceHash, _baseTransform);
        _attack03BounceRState = new FWaspAttack03BounceRState(_animator, _attack03BounceHash, _baseTransform);
        _attack03Fail01LState = new FWaspAttack03Fail01LState(_animator, _attack03Fail01Hash, _baseTransform);
        _attack03Fail01RState = new FWaspAttack03Fail01RState(_animator, _attack03Fail01Hash, _baseTransform);
        _attack03Success01LState = new FWaspAttack03Success01LState(_animator, _attack03Success01Hash, _baseTransform);
        _attack03Success01RState = new FWaspAttack03Success01RState(_animator, _attack03Success01Hash, _baseTransform);
        _attack03DeathLState = new FWaspAttack03DeathLState(_animator, _attack03Death01Hash, _baseTransform);
        _attack03DeathRState = new FWaspAttack03DeathRState(_animator, _attack03Death01Hash, _baseTransform);
        _attack04LState = new FWaspAttack04LState(_animator, _attack04Hash, _baseTransform);
        _attack04RState = new FWaspAttack04RState(_animator, _attack04Hash, _baseTransform);
        _attack04BounceLState = new FWaspAttack04BounceLState(_animator, _attack04BounceHash, _baseTransform);
        _attack04BounceRState = new FWaspAttack04BounceRState(_animator, _attack04BounceHash, _baseTransform);
        _attack04Fail01LState = new FWaspAttack04Fail01LState(_animator, _attack04Fail01Hash, _baseTransform);
        _attack04Fail01RState = new FWaspAttack04Fail01RState(_animator, _attack04Fail01Hash, _baseTransform);
        _attack04Success01LState = new FWaspAttack04Success01LState(_animator, _attack04Success01Hash, _baseTransform);
        _attack04Success01RState = new FWaspAttack04Success01RState(_animator, _attack04Success01Hash, _baseTransform);
        _attack04DeathLState = new FWaspAttack04DeathLState(_animator, _attack04Death01Hash, _baseTransform);
        _attack04DeathRState = new FWaspAttack04DeathRState(_animator, _attack04Death01Hash, _baseTransform);
    }

    private void StateMachineSetup()
    {
        // Enter
        At(_idleState, _enterLState, () => _isPlaying && _side == Side.Left);
        At(_idleState, _enterRState, () => _isPlaying && _side == Side.Right);
        // Enter to Attacks
        At(_enterLState, _attack01LState, IsAnimationEndedTwoOption01());
        At(_enterLState, _attack03LState, IsAnimationEndedTwoOption02());
        At(_enterRState, _attack01RState, IsAnimationEndedTwoOption01());
        At(_enterRState, _attack03RState, IsAnimationEndedTwoOption02());
        
        //---
        // Attack01 transitions
        // Bounce
        At(_attack01LState, _attack01BounceLState, IsCollidedWithLamp()); // TODO: make it based on collision instead
        At(_attack01RState, _attack01BounceRState, IsCollidedWithLamp());
        // Success
        At(_attack01BounceLState, _attack01Success01LState, IsAnimationEndedThreeOptionSuccess01());
        At(_attack01BounceLState, _attack01Success02LState, IsAnimationEndedThreeOptionSuccess02());
        At(_attack01BounceLState, _attack01Success03LState, IsAnimationEndedThreeOptionSuccess03());
        At(_attack01BounceRState, _attack01Success01RState, IsAnimationEndedThreeOptionSuccess01());
        At(_attack01BounceRState, _attack01Success02RState, IsAnimationEndedThreeOptionSuccess02());
        At(_attack01BounceRState, _attack01Success03RState, IsAnimationEndedThreeOptionSuccess03());
        // Fail
        At(_attack01BounceLState, _attack01Fail01LState, IsAnimationEndedFail());
        At(_attack01BounceRState, _attack01Fail01RState, IsAnimationEndedFail());
        // Death
        At(_attack01BounceLState, _attack01DeathLState, IsAnimationEndedDeath());
        At(_attack01BounceRState, _attack01DeathRState, IsAnimationEndedDeath());
        // Next Attack
        // - Success
        At(_attack01Success01LState, _attack01RState, IsAnimationEndedTwoOption01());
        At(_attack01Success01LState, _attack02RState, IsAnimationEndedTwoOption02());
        At(_attack01Success01RState, _attack01LState, IsAnimationEndedTwoOption01());
        At(_attack01Success01RState, _attack02LState, IsAnimationEndedTwoOption02());
        At(_attack01Success02LState, _attack04LState, IsAnimationEnded());
        At(_attack01Success02RState, _attack04RState, IsAnimationEnded());
        At(_attack01Success03LState, _attack02LState, IsAnimationEnded());
        At(_attack01Success03RState, _attack02RState, IsAnimationEnded());
        // - Fail
        At(_attack01Fail01LState, _attack02RState, IsAnimationEndedThreeOption01());
        At(_attack01Fail01LState, _attack03RState, IsAnimationEndedThreeOption02());
        At(_attack01Fail01LState, _attack04LState, IsAnimationEndedThreeOption03());
        At(_attack01Fail01RState, _attack02LState, IsAnimationEndedThreeOption01());
        At(_attack01Fail01RState, _attack03LState, IsAnimationEndedThreeOption02());
        At(_attack01Fail01RState, _attack04RState, IsAnimationEndedThreeOption03());

        //---       
        // Attack02 transitions
        // Bounce
        At(_attack02LState, _attack02BounceLState, IsCollidedWithLamp());
        At(_attack02RState, _attack02BounceRState, IsCollidedWithLamp());
        // Success
        At(_attack02BounceLState, _attack02Success01LState, IsAnimationEndedSuccess());
        At(_attack02BounceRState, _attack02Success01RState, IsAnimationEndedSuccess());
        // Fail
        At(_attack02BounceLState, _attack02Fail01LState, IsAnimationEndedTwoOptionFail01());
        At(_attack02BounceLState, _attack02Fail02LState, IsAnimationEndedTwoOptionFail02());
        At(_attack02BounceRState, _attack02Fail01RState, IsAnimationEndedTwoOptionFail01());
        At(_attack02BounceRState, _attack02Fail02RState, IsAnimationEndedTwoOptionFail02());
        // Death
        At(_attack02BounceLState, _attack02DeathLState, IsAnimationEndedDeath());
        At(_attack02BounceRState, _attack02DeathRState, IsAnimationEndedDeath());
        // Next Attack
        // - Success
        At(_attack02Success01LState, _attack02LState, IsAnimationEndedTwoOption01());
        At(_attack02Success01LState, _attack04LState, IsAnimationEndedTwoOption02());
        At(_attack02Success01RState, _attack02RState, IsAnimationEndedTwoOption01());
        At(_attack02Success01RState, _attack04RState, IsAnimationEndedTwoOption02());
        // - Fail
        At(_attack02Fail01LState, _attack01RState, IsAnimationEndedTwoOption01());
        At(_attack02Fail01LState, _attack03RState, IsAnimationEndedTwoOption02());
        At(_attack02Fail01RState, _attack01LState, IsAnimationEndedTwoOption01());
        At(_attack02Fail01RState, _attack03LState, IsAnimationEndedTwoOption02());
        At(_attack02Fail02LState, _attack02RState, IsAnimationEnded());
        At(_attack02Fail02RState, _attack02LState, IsAnimationEnded());
        
        //---
        // Attack03 transitions
        // Bounce
        At(_attack03LState, _attack03BounceLState, IsCollidedWithLamp());
        At(_attack03RState, _attack03BounceRState, IsCollidedWithLamp());
        // Success
        At(_attack03BounceLState, _attack03Success01LState, IsAnimationEndedSuccess());
        At(_attack03BounceRState, _attack03Success01RState, IsAnimationEndedSuccess());
        // Fail
        At(_attack03BounceLState, _attack03Fail01LState, IsAnimationEndedFail());
        At(_attack03BounceRState, _attack03Fail01RState, IsAnimationEndedFail());
        // Death
        At(_attack03BounceLState, _attack03DeathLState, IsAnimationEndedDeath());
        At(_attack03BounceRState, _attack03DeathRState, IsAnimationEndedDeath());
        // Next Attack
        // - Success
        At(_attack03Success01LState, _attack01LState, IsAnimationEndedTwoOption01());
        At(_attack03Success01LState, _attack03LState, IsAnimationEndedTwoOption02());
        At(_attack03Success01RState, _attack01RState, IsAnimationEndedTwoOption01());
        At(_attack03Success01RState, _attack03RState, IsAnimationEndedTwoOption02());
        // - Fail
        At(_attack03Fail01LState, _attack01RState, IsAnimationEndedTwoOption01());
        At(_attack03Fail01LState, _attack03RState, IsAnimationEndedTwoOption02());
        At(_attack03Fail01RState, _attack01LState, IsAnimationEndedTwoOption01());
        At(_attack03Fail01RState, _attack03LState, IsAnimationEndedTwoOption02());
        
        //---
        // Attack04 transitions
        // Bounce
        At(_attack04LState, _attack04BounceLState, IsCollidedWithLamp());
        At(_attack04RState, _attack04BounceRState, IsCollidedWithLamp());
        // Success
        At(_attack04BounceLState, _attack04Success01LState, IsAnimationEndedSuccess());
        At(_attack04BounceRState, _attack04Success01RState, IsAnimationEndedSuccess());
        // Fail
        At(_attack04BounceLState, _attack04Fail01LState, IsAnimationEndedFail());
        At(_attack04BounceRState, _attack04Fail01RState, IsAnimationEndedFail());
        // Death
        At(_attack04BounceLState, _attack04DeathLState, IsAnimationEndedDeath());
        At(_attack04BounceRState, _attack04DeathRState, IsAnimationEndedDeath());
        // Next Attack
        // - Success
        At(_attack04Success01LState, _attack01RState, IsAnimationEndedThreeOption01());
        At(_attack04Success01LState, _attack02LState, IsAnimationEndedThreeOption02());
        At(_attack04Success01LState, _attack03LState, IsAnimationEndedThreeOption03());
        At(_attack04Success01RState, _attack01LState, IsAnimationEndedThreeOption01());
        At(_attack04Success01RState, _attack02RState, IsAnimationEndedThreeOption02());
        At(_attack04Success01RState, _attack03RState, IsAnimationEndedThreeOption03());
        // - Fail
        At(_attack04Fail01LState, _attack01LState, IsAnimationEndedTwoOption01());
        At(_attack04Fail01LState, _attack03LState, IsAnimationEndedTwoOption02());
        At(_attack04Fail01RState, _attack01RState, IsAnimationEndedTwoOption01());
        At(_attack04Fail01RState, _attack03RState, IsAnimationEndedTwoOption02());
        
        // To Idle state on game over
        // L
        At(_attack01Fail01LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack01Success01LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack01Success02LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack01Success03LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack02Fail01LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack02Fail02LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack02Success01LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack03Fail01LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack03Success01LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack04Fail01LState, _idleState, IsAnimationEndedLampDeath());
        At(_attack04Success01LState, _idleState, IsAnimationEndedLampDeath());
        // R
        At(_attack01Fail01RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack01Success01RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack01Success02RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack01Success03RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack02Fail01RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack02Fail02RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack02Success01RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack03Fail01RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack03Success01RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack04Fail01RState, _idleState, IsAnimationEndedLampDeath());
        At(_attack04Success01RState, _idleState, IsAnimationEndedLampDeath());
        // To Idle state on death
        At(_attack01DeathLState, _idleState, IsAnimationEnded());
        At(_attack01DeathRState, _idleState, IsAnimationEnded());
        At(_attack02DeathLState, _idleState, IsAnimationEnded());
        At(_attack02DeathRState, _idleState, IsAnimationEnded());
        At(_attack03DeathLState, _idleState, IsAnimationEnded());
        At(_attack03DeathRState, _idleState, IsAnimationEnded());
        At(_attack04DeathLState, _idleState, IsAnimationEnded());
        At(_attack04DeathRState, _idleState, IsAnimationEnded());
        
        _stateMachine.SetState(_idleState);

        // Transition helper methods
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
        Func<bool> IsCollidedWithLamp() => () =>
        {
            if (_isCollided)
            {
                _isCollided = false;
                return true;
            }
            return false;
        };
        
        // Transition Predicates
        Func<bool> IsAnimationEnded() => () =>
        {
            if (_isAnimClipEnded && !_isLampDestroyed)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedSuccess() => () =>
        {
            if (_isAnimClipEnded && !_isDamaged && !_isDead)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedFail() => () =>
        {
            if (_isAnimClipEnded && _isDamaged && !_isDead)
            {
                _isAnimClipEnded = false;
                _isDamaged = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedDeath() => () =>
        {
            if (_isAnimClipEnded && _isDead)
            {
                _isAnimClipEnded = false;
                _isDead = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedLampDeath() => () =>
        {
            if (_isAnimClipEnded && _isLampDestroyed && !_isDead)
            {
                _isAnimClipEnded = false;
                _isLampDestroyed = false;
                _isPlaying = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedTwoOption01() => () =>
        {
            if (_isAnimClipEnded && _twoOptiosSplit == 0 && !_isLampDestroyed )
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedTwoOption02() => () =>
        {
            if (_isAnimClipEnded && _twoOptiosSplit == 1 && !_isLampDestroyed )
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedTwoOptionFail01() => () =>
        {
            if (_isAnimClipEnded && _twoOptiosSplit == 0 && _isDamaged && !_isDead)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedTwoOptionFail02() => () =>
        {
            if (_isAnimClipEnded && _twoOptiosSplit == 1 && _isDamaged && !_isDead)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedThreeOption01() => () =>
        {
            if (_isAnimClipEnded && _threeOptiosSplit == 0 && !_isLampDestroyed)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedThreeOption02() => () =>
        {
            if (_isAnimClipEnded && _threeOptiosSplit == 1 && !_isLampDestroyed)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedThreeOption03() => () =>
        {
            if (_isAnimClipEnded && _threeOptiosSplit == 2 && !_isLampDestroyed)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedThreeOptionSuccess01() => () =>
        {
            if (_isAnimClipEnded && _threeOptiosSplit == 0  && !_isDamaged && !_isDead)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedThreeOptionSuccess02() => () =>
        {
            if (_isAnimClipEnded && _threeOptiosSplit == 1 && !_isDamaged && !_isDead)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAnimationEndedThreeOptionSuccess03() => () =>
        {
            if (_isAnimClipEnded && _threeOptiosSplit == 2 && !_isDamaged && !_isDead)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
    }

    private void Update()
    {
        if (_isPlaying)
        {
            _stateMachine.Tick();
            _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FWasp", ""); // DEBUG
        }
    }

    private void LateUpdate()
    {
        // Correct position after animation - fix lamp penetrations
        if (ATTACK_STATES.Contains(_stateMachine.CurrentStateType))
        {
            // Check if lamp was penetrated
            Vector3 newPosition = transform.position;
            if ((newPosition - _lampPositionProvider.GetLampPosition()).magnitude < _colliderRadius + 0.5f)
            {
                newPosition = _lampPositionProvider.GetLampPosition() + newPosition.normalized * (0.5f + _colliderRadius);
            }
            transform.position = newPosition;
        }
    }
    
    
    // Event Handle Methods
    private void OnDeathStateEnded()
    {
        _isPlaying = false;
        DeathStateEnded?.Invoke();
    }

    private void OnBossAttackStarted()
    {
        BossAttackStarted?.Invoke();
    }
}
