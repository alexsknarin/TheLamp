using System.Collections.Generic;

public class EnemyQueue
{
    private List<EnemyTypes> _enemies;
    public int MaxEnemiesOnScreen { get; set; }
    public int AggressionLevel { get; set; }
    
    public EnemyQueue()
    {
        _enemies = new List<EnemyTypes>();
    }

    public void Clear()
    {
        _enemies.Clear();
    }
    
    public void Add(EnemyTypes enemy)
    {
        _enemies.Add(enemy);
    }
    
    public int Count()
    {
        return _enemies.Count;
    }

    public EnemyTypes Get(int index)
    {
        if (index < _enemies.Count)
        {
            return _enemies[index];    
        }
        else
        {
            return EnemyTypes.None;
        }
    }
}
