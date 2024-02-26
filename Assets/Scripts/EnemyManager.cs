using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("------ Enemy Prefabs -------")]
    [SerializeField] private GameObject _flyEnemyPrefab;
    private Enemy _fly;
    private bool _isAttacking = false;

    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += LampAttack;
    }
    
    private void OnDisable()
    {
        LampAttackModel.OnLampAttack -= LampAttack;
    }

    private void Start()
    {
        Init();
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
        }
        
    }

    private void Update()
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
