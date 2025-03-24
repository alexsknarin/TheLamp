namespace _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes
{
    [System.Serializable]
    public struct GlassDamageData
    {
        public int ImpactLastPointNumber;
        public int CracksNumberRight;
        public int CracksNumberLeft;
        public int CracksNumberBottom;
        public float CracksAmountRight;
        public float CracksAmountLeft;
        public float CracksAmountBottom;
        public LampDamagePoint LampDamagePoint01;
        public LampDamagePoint LampDamagePoint02;
        public LampDamagePoint LampDamagePoint03;
    }
}
