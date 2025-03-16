using System;
using UnityEngine;

public interface IStickyAttacker
{
    public event Action<Vector3, bool, string> StickyAttackEnded;
}
