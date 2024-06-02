[System.Serializable]
public class LampImpactPointsData
{
    public int ImpactLastPointNumber = 0;
    
    public float ImpactPoint01Strength = 0;
    public float ImpactPoint01LocalAngle = 0;
    public float ImpactPoint01GlobalAngle = 0;
    
    public float ImpactPoint02Strength = 0;
    public float ImpactPoint02LocalAngle = 0;
    public float ImpactPoint02GlobalAngle = 0;
    
    public float ImpactPoint03Strength = 0;
    public float ImpactPoint03LocalAngle = 0;
    public float ImpactPoint03GlobalAngle = 0;
    
    public void Reset()
    {
        ImpactLastPointNumber = 0;
        
        ImpactPoint01Strength = 0;
        ImpactPoint01LocalAngle = 0;
        ImpactPoint01GlobalAngle = 0;
        
        ImpactPoint02Strength = 0;
        ImpactPoint02LocalAngle = 0;
        ImpactPoint02GlobalAngle = 0;
        
        ImpactPoint03Strength = 0;
        ImpactPoint03LocalAngle = 0;
        ImpactPoint03GlobalAngle = 0;
    }
}
