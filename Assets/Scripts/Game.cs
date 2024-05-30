using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Lamp _lamp;
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private LampStatsManager _lampStatsManager;
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private UGSSetup _ugsSetup;
    [SerializeField] private ScoresManager _scoresManager;
    [SerializeField] private SaveLoadManager _saveLoadManager;
    [SerializeField] private AdsManager _adsManager;
    [SerializeField] private GameStates _currentGameState;
    [SerializeField] private bool _skipIntro;
    [SerializeField] private float _introDuration;
    [SerializeField] private float _deathDuration;


    // State paremeters  
    private bool _isLampDead = false;
    
    private readonly bool GAME_RESET = true;
    private readonly bool GAME_RUNNING = false;
    private readonly bool SAVE_UPGRADES = true;
    private readonly bool DONT_SAVE_UPGRADES = false;


    private void OnEnable()
    {
        _googleSheetsDataReader.OnDataLoaded += InitializeEnemyManager;
        _uiManager.OnIntroFinished += OnIntroEnded;
        _ugsSetup.OnConsentAddressed += HandleDataConsentAddressed;
        PlayerInputHandler.OnPlayerAttack += HandlePlayerAttackButtonPressed;
        EnemyManager.OnWaveEnded += HandleWaveEnded;
        Lamp.OnLampDead += HandleLampDead;
        _lampStatsManager.OnHealthChange += HandleStatsUpgrade;
        _lampStatsManager.OnCooldownChange += HandleStatsUpgrade;
        _adsManager.OnAdFinished += SaveRewards;
        _uiManager.OnGameoverFinished += HandleGameoverUiAnimationFinished;
    }

    private void OnDisable()
    {
        _googleSheetsDataReader.OnDataLoaded -= InitializeEnemyManager;
        _uiManager.OnIntroFinished -= OnIntroEnded;
        _ugsSetup.OnConsentAddressed -= HandleDataConsentAddressed;
        PlayerInputHandler.OnPlayerAttack -= HandlePlayerAttackButtonPressed;
        EnemyManager.OnWaveEnded -= HandleWaveEnded;
        Lamp.OnLampDead -= HandleLampDead;
        _lampStatsManager.OnHealthChange -= HandleStatsUpgrade;
        _lampStatsManager.OnCooldownChange -= HandleStatsUpgrade;
        _adsManager.OnAdFinished -= SaveRewards;
        _uiManager.OnGameoverFinished -= HandleGameoverUiAnimationFinished;
    }
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        
        if (_skipIntro)
        {
            _introDuration = 0.001f;
        }
        
        _isLampDead = false;
        _currentGameState = GameStates.Loading;

        // Init all systems
        _saveLoadManager.Initialize();
        // Load game state
        _saveLoadManager.LoadGame();
        _scoresManager.Initialize();
        _lamp.Initialize();
        _adsManager.Initialize();
        
        _uiManager.SetIntroDuration(_introDuration);
        _uiManager.Initialize();
        _playerInputHandler.Initialize();
        _googleSheetsDataReader.Initialize(); // This Method will trigger InitializeEnemyManager and  Switch State
    }

    public void RestartGame(int mode)
    {
        
        if (mode == 0)
        {
            // Save Upgrades AFTER add is finished
            _adsManager.ShowAd();
        }
        else
        {
            // Don't Save Upgrades
            _saveLoadManager.SaveGame(GAME_RESET, DONT_SAVE_UPGRADES);
            
            _isLampDead = false;
            _currentGameState = GameStates.Loading;
            _enemyManager.Restart();
            _uiManager.Initialize();
            _playerInputHandler.Initialize();
            _scoresManager.Initialize();
            _lamp.Initialize();
            _googleSheetsDataReader.Initialize(); // This Method will trigger InitializeEnemyManager and  Switch State
        }
    }
    
    private void SaveRewards()
    {
        _saveLoadManager.SaveGame(GAME_RESET, SAVE_UPGRADES);
        
        _isLampDead = false;
        _currentGameState = GameStates.Loading;
        _enemyManager.Restart();
        _uiManager.Initialize();
        _playerInputHandler.Initialize();
        _scoresManager.Initialize();
        _lamp.Initialize();
        _googleSheetsDataReader.Initialize(); // This Method will trigger InitializeEnemyManager and  Switch State
    }
    
    private void InitializeEnemyManager()
    {
        SwitchGameState();

        // Initialize enemyManager only after we 100% sure that data was initialized
        _enemyManager.Initialize();
        if (PlayerPrefs.HasKey("dataConsent"))
        {
            _ugsSetup.Setup();
            SwitchGameState();
        }
    }
    
    // Player Agrees or Disagrees with Data Collection
    private void HandleDataConsentAddressed()
    {
        SwitchGameState();
    }
    private void OnIntroEnded()
    {
        SwitchGameState();
    }
    private void HandlePlayerAttackButtonPressed()
    {
        if (_currentGameState == GameStates.Prepare)
        {
            SwitchGameState();
        }
    }
    
    private void HandleStatsUpgrade()
    {
        _saveLoadManager.SaveGame(GAME_RUNNING, SAVE_UPGRADES);
    }
    
    private void HandleWaveEnded(int waveNumber)
    {
        SwitchGameState();
    }
    
    private void HandleLampDead(EnemyBase enemy)
    {
        _isLampDead = true;
        SwitchGameState();
    }
    
    private void HandleGameoverUiAnimationFinished()
    {
        _enemyManager.DeactivateAllEnemies();
    }
   
    private void SwitchGameState()
    {
        switch (_currentGameState)
        {
            case GameStates.Loading:
                _currentGameState = GameStates.ConsentScreen;
                break;
            case GameStates.ConsentScreen:
                _currentGameState = GameStates.Intro;
                _uiManager.PlayIntro();
                _lamp.PlayIntro(_introDuration);
                break;
            case GameStates.Intro:
                _playerInputHandler.EnableAttackInput();
                _uiManager.StartPrepare(_enemyManager.CurrentWave);
                _currentGameState = GameStates.Prepare;
                break;
            case GameStates.Prepare:
                _currentGameState = GameStates.Fight;
                _uiManager.StartFight();
                _enemyManager.StartWave();
                break;
            case GameStates.Fight:
                if (_isLampDead)
                {
                    _saveLoadManager.SaveTempData();
                    _playerInputHandler.DisableAttackInput();
                    _lamp.PlayDeath(_deathDuration);
                    _enemyManager.HandleGameOver();
                    _uiManager.StartGameOver();
                    _currentGameState = GameStates.GameOver;   
                }
                else
                {
                    _saveLoadManager.SaveGame(GAME_RUNNING, SAVE_UPGRADES);
                    _uiManager.StartPrepare(_enemyManager.CurrentWave);
                    _currentGameState = GameStates.Prepare;    
                }
                break;
        }
    }

    public void ExitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
        
#if UNITY_ANDROID
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    
}
