using UnityEngine;

namespace _GAME.Scripts.Lib
{
    public class HierarchyUtilities
    {
        public static void ParentWithoutOffset(
            Transform currentTransform, 
            Transform parentTransform
            )
        {
            currentTransform.SetParent(parentTransform, false);
            currentTransform.localPosition = Vector3.zero;
            currentTransform.localRotation = Quaternion.identity;
        }
    }
}
