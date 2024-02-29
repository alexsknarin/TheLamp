using System.Collections.Generic;

public class SpawnQueue
{
    private List<EnemyQueue> _enemyQueues;
    
    public SpawnQueue()
    {
        _enemyQueues = new List<EnemyQueue>();
    }
    
    public void Clear()
    {
        _enemyQueues.Clear();
    }
    
    public void Add(EnemyQueue enemyQueue)
    {
        _enemyQueues.Add(enemyQueue);
    }
    
    public EnemyQueue Get(int index)
    {
        if (index < _enemyQueues.Count)
        {
            return _enemyQueues[index];    
        }
        else
        {
            return null;
        }
    }
    
    public int Count()
    {
        return _enemyQueues.Count;
    }
}
