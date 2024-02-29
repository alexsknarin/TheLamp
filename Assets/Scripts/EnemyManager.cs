using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour,IInitializable
{
    [Header("------ Enemy Prefabs -------")]
    [SerializeField] private GameObject _flyEnemyPrefab;
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    private SpawnQueueGenerator _spawnQueueGenerator;
    private Enemy _fly;
    private bool _isAttacking = false;
    private bool _isInitialized = false;
    
    public static event Action OnEnemyDamaged;

    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += LampAttack;
    }
    
    private void OnDisable()
    {
        LampAttackModel.OnLampAttack -= LampAttack;
    }
    
    public void Initialize()
    {
        Init();
        Debug.Log("Enemy Manager initialized");
        _spawnQueueGenerator = new SpawnQueueGenerator(_googleSheetsDataReader.SheetData);
        var tmp = _spawnQueueGenerator.Generate();
        
        // Check SpawnQueueGenerator.cs for the implementation of the Generate method
        int spawnCount = tmp.Count();
        for (int i=0; i<spawnCount; i++)
        {
            string result = "Wave: " + (i+1).ToString() + " ";
            var wave = tmp.Get(i);
            for(int j=0; j<wave.Count(); j++)
            {
                result += wave.Get(j).ToString() + " ";
            }
            
            Debug.Log(result);
        }
        
        
        
        
        _isInitialized = true;
    }
    

    public void Init()
    {
        SpawnEnemy();    
    }
    
    public void SpawnEnemy()
    {
        var enemy = Instantiate(_flyEnemyPrefab, transform.position, Quaternion.identity);
        _fly = enemy.gameObject.GetComponent<Enemy>();
        _fly.Initialize();
    }

    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        // for each active enemy - get the distance to the lamp - need to cache it in enbemy class later
        Vector3 currentEnemyPosition = _fly.transform.position;
        currentEnemyPosition.z = 0;
        if(currentEnemyPosition.magnitude < attackDistance)
        {
            _fly.RecieveDamage(attackPower);
            OnEnemyDamaged?.Invoke();
        }
        
    }

    private void Update()
    {
        if (_isInitialized)
        {
            if(!_isAttacking)
            {
                float random = Random.Range(0f, 1f);
           
                if (random > 0.973f)
                {
                    _fly.Attack();
                }
            }    
        }
    }


}
