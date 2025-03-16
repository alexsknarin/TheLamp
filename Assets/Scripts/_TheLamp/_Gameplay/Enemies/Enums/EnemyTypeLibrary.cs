using System;
using System.Collections.Generic;

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