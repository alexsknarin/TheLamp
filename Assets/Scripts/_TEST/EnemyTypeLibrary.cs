using System;
using System.Collections.Generic;

public class EnemyTypeLibrary
{
    public static Dictionary<EnemyType, Type> EnemyTypeDictionary = new Dictionary<EnemyType, Type>
    {
        { EnemyType.Fly, typeof(FFly) },
        { EnemyType.Moth, typeof(FMoth) },
        { EnemyType.Firefly, typeof(FFireFly) },
        { EnemyType.Ladybug, typeof(FLadybug) },
        { EnemyType.Spider, typeof(FSpider) },
        { EnemyType.Mothling, typeof(FMothling) }
    };
    
    public static Dictionary<Type, EnemyType> TypeEnemyDictionary = new Dictionary<Type, EnemyType>
    {
        { typeof(FFly), EnemyType.Fly },
        { typeof(FMoth), EnemyType.Moth },
        { typeof(FFireFly), EnemyType.Firefly },
        { typeof(FLadybug), EnemyType.Ladybug },
        { typeof(FSpider), EnemyType.Spider },
        { typeof(FMothling), EnemyType.Mothling }
    };
}