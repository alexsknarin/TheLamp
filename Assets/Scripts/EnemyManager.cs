using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("------ Enemy Prefabs -------")]
    [SerializeField] private GameObject _flyEnemyPrefab;
    private FlyMovement _flyMovement;
    private bool _isAttacking = false;

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
        _flyMovement = enemy.gameObject.GetComponent<FlyMovement>();
        _flyMovement.Init();
    }

    private void Update()
    {
        if(!_isAttacking)
        {
            float random = Random.Range(0f, 1f);
           
            if (random > 0.95f)
            {
                _flyMovement.StartAttack();
            }
        }
    }
}
