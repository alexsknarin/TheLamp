using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("------ Enemy Prefabs -------")]
    [SerializeField] private GameObject _flyEnemyPrefab;
    private Enemy _fly;
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
        _fly = enemy.gameObject.GetComponent<Enemy>();
        _fly.Initialize();
    }

    private void Update()
    {
        if(!_isAttacking)
        {
            float random = Random.Range(0f, 1f);
           
            if (random > 0.95f)
            {
                _fly.Attack();
            }
        }
    }
}
