using System;
using System.Collections.Generic;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Enemies.DragonflyProjectileMoth;
using _GAME.Scripts.Enemies.DragonflyProjectileSpider;
using _GAME.Scripts.Enemies.FireFly;
using _GAME.Scripts.Enemies.Fly;
using _GAME.Scripts.Enemies.Ladybug;
using _GAME.Scripts.Enemies.Megabeetle;
using _GAME.Scripts.Enemies.Megamothling;
using _GAME.Scripts.Enemies.Moth;
using _GAME.Scripts.Enemies.Mothling;
using _GAME.Scripts.Enemies.Spider;
using _GAME.Scripts.Enemies.Wasp;

namespace _GAME.Scripts.Lib.Enums
{
    public class EnemyTypeLibrary
    {
        public static Dictionary<EnemyType, Type> EnemyTypeDictionary = new Dictionary<EnemyType, Type>
        {
            { EnemyType.Fly, typeof(Fly) },
            { EnemyType.Moth, typeof(Moth) },
            { EnemyType.Firefly, typeof(FireFly) },
            { EnemyType.Ladybug, typeof(Ladybug) },
            { EnemyType.Spider, typeof(Spider) },
            { EnemyType.Mothling, typeof(Mothling) },
            { EnemyType.Megamothling , typeof(Megamothling) },
            { EnemyType.Wasp , typeof(Wasp) },
            { EnemyType.Megabeetle , typeof(Megabeetle) },
            { EnemyType.Dragonfly , typeof(Dragonfly) },
            { EnemyType.DragonflyProjectileSpider , typeof(DragonflyProjectileSpider) },
            { EnemyType.DragonflyProjectileMoth , typeof(DragonflyProjectileMoth) }
        
        };
    
        public static Dictionary<Type, EnemyType> TypeEnemyDictionary = new Dictionary<Type, EnemyType>
        {
            { typeof(Fly), EnemyType.Fly },
            { typeof(Moth), EnemyType.Moth },
            { typeof(FireFly), EnemyType.Firefly },
            { typeof(Ladybug), EnemyType.Ladybug },
            { typeof(Spider), EnemyType.Spider },
            { typeof(Mothling), EnemyType.Mothling },
            { typeof(Megamothling), EnemyType.Megamothling },
            { typeof(Wasp), EnemyType.Wasp },
            { typeof(Megabeetle), EnemyType.Megabeetle },
            { typeof(Dragonfly), EnemyType.Dragonfly },
            { typeof(DragonflyProjectileSpider), EnemyType.DragonflyProjectileSpider },
            { typeof(DragonflyProjectileMoth), EnemyType.DragonflyProjectileMoth }
        };
    }
}