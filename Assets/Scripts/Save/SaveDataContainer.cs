using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataContainer", menuName = "SaveDataContainer")]
public class SaveDataContainer : ScriptableObject
{
    public int Wave;
    public int CurrentScore;
    public int Level;
    public int UpgradePoints;
    public int UpgradePointsThreshold;
    public int UpgradeThesholdIncrement;
    public int MaxHealth;
    public int Health;
    public float CooldownTime;
    public int LampDamageWeightRight;
    public int LampDamageWeightLeft;
    public int LampDamageWeightBottom;
    public float AttackDistance;

    public int ImpactLastPointNumber;
    
    public float ImpactPoint01Strength;
    public float ImpactPoint01LocalAngle;
    public float ImpactPoint01GlobalAngle;
    
    public float ImpactPoint02Strength;
    public float ImpactPoint02LocalAngle;
    public float ImpactPoint02GlobalAngle;
    
    public float ImpactPoint03Strength;
    public float ImpactPoint03LocalAngle;
    public float ImpactPoint03GlobalAngle;
    
    private SaveData _saveData = new SaveData();

    public void SetData(SaveData saveData)
    {
        Wave = saveData.Wave;
        CurrentScore = saveData.CurrentScore;
        Level = saveData.Level;
        UpgradePoints = saveData.UpgradePoints;
        UpgradePointsThreshold = saveData.UpgradePointsThreshold;
        UpgradeThesholdIncrement = saveData.UpgradeThesholdIncrement;
        MaxHealth = saveData.MaxHealth;
        Health = saveData.Health;
        CooldownTime = saveData.CooldownTime;
        AttackDistance = saveData.AttackDistance;
        
        LampDamageWeightRight = saveData.LampDamageWeightRight;
        LampDamageWeightLeft = saveData.LampDamageWeightLeft;
        LampDamageWeightBottom = saveData.LampDamageWeightBottom;
        
        ImpactLastPointNumber = saveData.ImpactLastPointNumber;
        
        ImpactPoint01Strength = saveData.ImpactPoint01Strength;
        ImpactPoint01LocalAngle = saveData.ImpactPoint01LocalAngle;
        ImpactPoint01GlobalAngle = saveData.ImpactPoint01GlobalAngle;
        
        ImpactPoint02Strength = saveData.ImpactPoint02Strength;
        ImpactPoint02LocalAngle = saveData.ImpactPoint02LocalAngle;
        ImpactPoint02GlobalAngle = saveData.ImpactPoint02GlobalAngle;
        
        ImpactPoint03Strength = saveData.ImpactPoint03Strength;
        ImpactPoint03LocalAngle = saveData.ImpactPoint03LocalAngle;
        ImpactPoint03GlobalAngle = saveData.ImpactPoint03GlobalAngle;
    }
    
    public SaveData GetData()
    {
        _saveData.Wave = Wave;
        _saveData.CurrentScore = CurrentScore;
        _saveData.Level = Level;
        _saveData.UpgradePoints = UpgradePoints;
        _saveData.UpgradePointsThreshold = UpgradePointsThreshold;
        _saveData.UpgradeThesholdIncrement = UpgradeThesholdIncrement;
        _saveData.MaxHealth = MaxHealth;
        _saveData.Health = Health;
        _saveData.CooldownTime = CooldownTime;
        _saveData.AttackDistance = AttackDistance;
        
        _saveData.LampDamageWeightRight = LampDamageWeightRight;
        _saveData.LampDamageWeightLeft = LampDamageWeightLeft;
        _saveData.LampDamageWeightBottom = LampDamageWeightBottom;
        
        _saveData.ImpactLastPointNumber = ImpactLastPointNumber;
        _saveData.ImpactPoint01Strength = ImpactPoint01Strength;
        _saveData.ImpactPoint01LocalAngle = ImpactPoint01LocalAngle;
        _saveData.ImpactPoint01GlobalAngle = ImpactPoint01GlobalAngle;
        _saveData.ImpactPoint02Strength = ImpactPoint02Strength;
        _saveData.ImpactPoint02LocalAngle = ImpactPoint02LocalAngle;
        _saveData.ImpactPoint02GlobalAngle = ImpactPoint02GlobalAngle;
        _saveData.ImpactPoint03Strength = ImpactPoint03Strength;
        _saveData.ImpactPoint03LocalAngle = ImpactPoint03LocalAngle;
        _saveData.ImpactPoint03GlobalAngle = ImpactPoint03GlobalAngle;
        return _saveData;
    }
}
