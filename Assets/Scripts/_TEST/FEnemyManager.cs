using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FEnemyManager : MonoBehaviour
{
    // TODO: move this to the Game Context Root
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    [SerializeField] private LampStickyDetectionService _lampStickyDetectionService;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;
    private CollidableEnemy _enemyMothling;
    private CollidableEnemy _enemyMoth;
    private CollidableEnemy _enemyFly;
    private CollidableEnemy _enemyFireFly;
    private CollidableEnemy _enemySpider;
    private FEnemy _enemyLadybug;
        

    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    private FlyMovementStateFactory _flyMovementStateFactory;
    private MothMovementStateFactory _mothMovementStateFactory;
    private SpiderMovementStateFactory _spiderMovementStateFactory;
    private LadybugMovementStateFactory _ladybugMovementStateFactory;
    
    private FEnemyFactory _enemyFactory;
    private FEnemyPool _enemyPool;
    
    private List<IDamageable> _damageables = new();
    private List<IDamageable> _damageablesToRemove = new();

    private void Awake()
    {
        _mothlingMovementStateFactory = new MothlingMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _flyMovementStateFactory = new FlyMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _mothMovementStateFactory = new MothMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _spiderMovementStateFactory = new SpiderMovementStateFactory();
        _ladybugMovementStateFactory = new LadybugMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        
        _enemyFactory = new FEnemyFactory(
            _mothlingMovementStateFactory, 
            _flyMovementStateFactory, 
            _mothMovementStateFactory, 
            _spiderMovementStateFactory,
            _ladybugMovementStateFactory,
            _lampPositionProviderService
            );
        
        _enemyPool = new FEnemyPool(_enemyFactory);
        _enemyPool.Initialize();
        
        _lampCollisionDetectionService.Initialize();
        _lampStickyDetectionService.Initialize();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            _enemyPool.PreloadEnemy(typeof(FMoth));
            _enemyPool.PreloadEnemy(typeof(FMothling));
            _enemyPool.PreloadEnemy(typeof(FFly));
            _enemyPool.PreloadEnemy(typeof(FFireFly));
            _enemyPool.PreloadEnemy(typeof(FSpider));
            _enemyPool.PreloadEnemy(typeof(FLadybug));
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            _enemyMothling = (FMothling)_enemyPool.Get(typeof(FMothling));
            _enemyMothling.Play();
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            _enemyMoth = (FMoth)_enemyPool.Get(typeof(FMoth));
            _enemyMoth.Play();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            _enemyFly = (FFly)_enemyPool.Get(typeof(FFly));
            _enemyFly.Play();
        }
        
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            _enemyFireFly = (FFireFly)_enemyPool.Get(typeof(FFireFly));
            _enemyFireFly.Play();
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            _enemySpider = (FSpider)_enemyPool.Get(typeof(FSpider));
            _enemySpider.Play();
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            _enemyLadybug = (FLadybug)_enemyPool.Get(typeof(FLadybug));
            _enemyLadybug.Play();
            _lampStickyDetectionService.AddStickable((FLadybug)_enemyLadybug);
            _damageables.Add(_enemyLadybug);
        }

        // Start Enemy Attack
        if (Input.GetKeyDown(KeyCode.A))
        {
            // if (_enemyMothling.IsReadyToAttack)
            // {
            //     _lampCollisionDetectionService.AddCollidable(_enemyMothling);
            //     _damageables.Add(_enemyMothling);
            //     _enemyMothling.Attack();    
            // }
            // if (_enemyMoth.IsReadyToAttack)
            // {
            //     _lampCollisionDetectionService.AddCollidable(_enemyMoth);
            //     _damageables.Add(_enemyMoth);
            //     _enemyMoth.Attack();    
            // }
            // if (_enemyFly.IsReadyToAttack)
            // {
            //     _lampCollisionDetectionService.AddCollidable(_enemyFly);
            //     _damageables.Add(_enemyFly);
            //     _enemyFly.Attack();    
            // }
            // if (_enemyFireFly.IsReadyToAttack)
            // {
            //     _lampCollisionDetectionService.AddCollidable(_enemyFireFly);
            //     _damageables.Add(_enemyFireFly);
            //     _enemyFireFly.Attack();    
            // }
            if (_enemySpider.IsReadyToAttack)
            {
                _lampCollisionDetectionService.AddCollidable(_enemySpider);
                _damageables.Add(_enemySpider);
                _enemySpider.Attack();    
            }
            
        }
        
        // Emulate enemy Death
        if (Input.GetKeyDown(KeyCode.D))
        {
            _lampCollisionDetectionService.RemoveCollidable(_enemyMothling);
            _enemyMothling.DoDeath();
        }
        
        // Emulate enemy Spread
        if (Input.GetKeyDown(KeyCode.S))
        {
            // _lampCollisionDetectionService.RemoveCollidable(_enemyMothling);
            // _lampCollisionDetectionService.RemoveCollidable(_enemyMoth);
            // _lampCollisionDetectionService.RemoveCollidable(_enemyFly);
            // _lampCollisionDetectionService.RemoveCollidable(_enemyFireFly);
            // _enemyMothling.Spread();
            // _enemyMoth.Spread();
            // _enemyFly.Spread();
            // _enemyFireFly.Spread();
            
            // _lampCollisionDetectionService.RemoveCollidable(_enemySpider);
            // _enemySpider.Spread();
            
            _enemyLadybug.Spread();
        }
        
        // Do Lamp Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (_damageables.Count != 0)
            {
                _damageablesToRemove.Clear();
                
                foreach (var damageable in _damageables)
                {
                    if (damageable.IsReadyForDamage)
                    {
                        damageable.ReceiveDamage(3);
                        
                        if (!(damageable is FLadybug))
                        {
                            Debug.Log("Non ladybug marked for removal");
                            _damageablesToRemove.Add(damageable);
                        }
                        if ((damageable is FLadybug) && damageable.IsDead)
                        {
                            Debug.Log("Ladybug marked for removal because it is dead");
                            _damageablesToRemove.Add(damageable);
                        }
                    }
                }
                
                foreach (var damageable in _damageablesToRemove)
                {
                    Debug.Log("Removing damageables");
                    _damageables.Remove(damageable);
                }
            }
            
            Debug.Log("Damageables count: " + _damageables.Count);
        }
        
    }
}
