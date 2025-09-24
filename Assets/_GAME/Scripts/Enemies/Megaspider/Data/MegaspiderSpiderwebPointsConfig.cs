using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.Data
{
    [CreateAssetMenu(fileName = "MegaspiderSpiderwebPointsConfig", menuName = "Configs/MegaspiderSpiderwebPointsConfig")]
    public class MegaspiderSpiderwebPointsConfig : ScriptableObject
    {
        [field:Header("Enter 01")]
        [field:SerializeField] public Vector3 Enter01LStartPoint { get; private set; } = new(-4.74f, 2.61f, 7.15f);
        [field:SerializeField] public Vector3 Enter01LEndPoint { get; private set; } = new(2.71f, 1.76f, 1.76f);
        [field:SerializeField] public Vector3 Enter01RStartPoint { get; private set; } = new(4.74f, 2.61f, 7.15f);
        [field:SerializeField] public Vector3 Enter01REndPoint { get; private set; } = new(-2.71f, 1.76f, 1.76f);
        [field:Header("Enter 02")]
        [field:SerializeField] public Vector3 Enter02LStartPoint { get; private set; } = new(2.71f, 1.55f, 0.69f);
        [field:SerializeField] public Vector3 Enter02LEndPoint { get; private set; } = new(-1.9f, 1.76f, -1.2f);
        [field:SerializeField] public Vector3 Enter02RStartPoint { get; private set; } = new(-2.71f, 1.55f, 0.69f);
        [field:SerializeField] public Vector3 Enter02REndPoint { get; private set; } = new(1.9f, 1.76f, -1.2f);
        [field:Header("Enter Hang")]
        [field:SerializeField] public Vector3 EnterHangLStartPoint { get; private set; } = new(-0.35f, 1.05f, -3.99f);
        [field:SerializeField] public Vector3 EnterHangRStartPoint { get; private set; } = new(0.35f, 1.05f, -3.99f);
        [field:Header("Projectile Bottom")]
        [field:SerializeField] public Vector3 ProjectileBottomAttackLStartPoint { get; private set; } = new(-1.93f, -2.34f, -1.34f);
        [field:SerializeField] public Vector3 ProjectileBottomAttackLEndPoint { get; private set; } = new(2.46f, -1.52f, 0.54f);
        [field:SerializeField] public Vector3 ProjectileBottomAttackRStartPoint { get; private set; } = new(1.93f, -2.34f, -1.34f);
        [field:SerializeField] public Vector3 ProjectileBottomAttackREndPoint { get; private set; } = new(-2.46f, -1.52f, 0.54f);
        [field:Header("Projectile Double Down 01")]
        [field:SerializeField] public Vector3 ProjectileDoubleDown01LStartPoint { get; private set; } = new(-3.28f, 2.6f, 1.02f);
        [field:SerializeField] public Vector3 ProjectileDoubleDown01LEndPoint { get; private set; } = new(2.67f, 1.38f, 0.71f);
        [field:SerializeField] public Vector3 ProjectileDoubleDown01RStartPoint { get; private set; } = new(3.28f, 2.6f, 1.02f);
        [field:SerializeField] public Vector3 ProjectileDoubleDown01REndPoint { get; private set; } = new(-2.67f, 1.38f, 0.71f);
        [field:Header("Projectile Double Down 02")]
        [field:SerializeField] public Vector3 ProjectileDoubleDown02LStartPoint { get; private set; } = new(2.46f, -1.69f, 0.28f);
        [field:SerializeField] public Vector3 ProjectileDoubleDown02LEndPoint { get; private set; } = new(-1.93f, -2.87f, -1.68f);
        [field:SerializeField] public Vector3 ProjectileDoubleDown02RStartPoint { get; private set; } = new(-2.46f, -1.69f, 0.28f);
        [field:SerializeField] public Vector3 ProjectileDoubleDown02REndPoint { get; private set; } = new(1.93f, -2.87f, -1.68f);
        [field:Header("Projectile Double Up 01")]
        [field:SerializeField] public Vector3 ProjectileDoubleUp01LStartPoint { get; private set; } = new(-1.93f, -2.87f, -1.68f);
        [field:SerializeField] public Vector3 ProjectileDoubleUp01LEndPoint { get; private set; } = new(2.46f, -1.69f, 0.28f);
        [field:SerializeField] public Vector3 ProjectileDoubleUp01RStartPoint { get; private set; } = new(1.93f, -2.87f, -1.68f);
        [field:SerializeField] public Vector3 ProjectileDoubleUp01REndPoint { get; private set; } = new(-2.46f, -1.69f, 0.28f);
        [field:Header("Projectile Double Up 02")]
        [field:SerializeField] public Vector3 ProjectileDoubleUp02LStartPoint { get; private set; } = new(2.67f, 1.04f, 0.56f);
        [field:SerializeField] public Vector3 ProjectileDoubleUp02LEndPoint { get; private set; } = new(-3.12f, 2.43f, 0.6f);
        [field:SerializeField] public Vector3 ProjectileDoubleUp02RStartPoint { get; private set; } = new(-2.67f, 1.04f, 0.56f);
        [field:SerializeField] public Vector3 ProjectileDoubleUp02REndPoint { get; private set; } = new(3.12f, 2.43f, 0.6f);
        [field:Header("Projectile Top")]
        [field:SerializeField] public Vector3 ProjectileTopAttackLStartPoint { get; private set; } = new(-2.71f, 1.61f, 1.27f);
        [field:SerializeField] public Vector3 ProjectileTopAttackLEndPoint { get; private set; } = new(2.92f, 3.13f, 1.65f);
        [field:SerializeField] public Vector3 ProjectileTopAttackRStartPoint { get; private set; } = new(2.71f, 1.61f, 1.27f);
        [field:SerializeField] public Vector3 ProjectileTopAttackREndPoint { get; private set; } = new(-2.92f, 3.13f, 1.65f);
        [field:Header("Zigzag 01")]
        [field:SerializeField] public Vector3 ZigzagAttack01LStartPoint { get; private set; } = new(-1.09f, -1.67f, -3.75f);
        [field:SerializeField] public Vector3 ZigzagAttack01LEndPoint { get; private set; } = new(1.76f, -1.35f, -2.05f);
        [field:SerializeField] public Vector3 ZigzagAttack01RStartPoint { get; private set; } = new(1.09f, -1.67f, -3.75f);
        [field:SerializeField] public Vector3 ZigzagAttack01REndPoint { get; private set; } = new(-1.76f, -1.35f, -2.05f);
        [field:Header("Zigzag 02")]
        [field:SerializeField] public Vector3 ZigzagAttack02LStartPoint { get; private set; } = new(1.76f, -1.35f, -2.05f);
        [field:SerializeField] public Vector3 ZigzagAttack02LEndPoint { get; private set; } = new(-1.95f, -0.78f, -1.24f);
        [field:SerializeField] public Vector3 ZigzagAttack02RStartPoint { get; private set; } = new(-1.76f, -1.35f, -2.05f);
        [field:SerializeField] public Vector3 ZigzagAttack02REndPoint { get; private set; } = new(1.95f, -0.78f, -1.24f);
        [field:Header("Zigzag 03")]
        [field:SerializeField] public Vector3 ZigzagAttack03LStartPoint { get; private set; } = new(-1.95f, -0.78f, -1.24f);
        [field:SerializeField] public Vector3 ZigzagAttack03LEndPoint { get; private set; } = new(5.20f, 2.74f, 9.76f);
        [field:SerializeField] public Vector3 ZigzagAttack03RStartPoint { get; private set; } = new(1.95f, -0.78f, -1.24f);
        [field:SerializeField] public Vector3 ZigzagAttack03REndPoint { get; private set; } = new(-5.20f, 2.74f, 9.76f);
        [field:Header("Hang Attack")]
        [field:SerializeField] public Vector3 HangAttackLStartPoint { get; private set; } = new(-0.885392f, 3.38f, 0f);
        [field:SerializeField] public Vector3 HangAttackRStartPoint { get; private set; } = new(0.885392f, 3.38f, 0f);
    }
}
