using _GAME.Scripts.Enemies;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

// TODO: Add to namespace, fix readonly
public class SpiderPositionHolder: ISpiderSpawnAvailablityProvider, ISpiderSideDirectionProvider
{
    private readonly int[] _directions = {-1, 1};
    private EnemyMovementBase[] _occupants = { null, null };
    private bool[] _occupiedStatus = { false, false };

    public void Reset()
    {
        _occupants[0] = null;
        _occupants[1] = null;
        _occupiedStatus[0] = false;
        _occupiedStatus[1] = false;
    }

    public bool CheckPointAvailability()
    {
        if (_occupiedStatus[0] && _occupiedStatus[1])
        {
            return false;
        }
        return true;
    }

    public int RequestPoint(EnemyMovementBase occupant)
    {
        int side = 0;
        if (!_occupiedStatus[0] && !_occupiedStatus[1])
        {
            side = Random.Range(0, 2);
        }
        else if (_occupiedStatus[0] && !_occupiedStatus[1])
        {
            side = 1;
        }
        else if (!_occupiedStatus[0] && _occupiedStatus[1])
        {
            side = 0;
        }
        
        _occupants[side] = occupant;
        _occupiedStatus[side] = true;
        return _directions[side];
    }

    public void ReleasePoint(EnemyMovementBase occupant)
    {
        for (int i = 0; i < _occupants.Length; i++)
        {
            if (_occupants[i] == occupant)
            {
                _occupants[i] = null;
                _occupiedStatus[i] = false;
                break;
            }
        }
    }
}
