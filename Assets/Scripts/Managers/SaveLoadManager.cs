using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour, IInitializable
{
    [SerializeField] private SaveDataContainer _saveDataContainer;
    private SaveData _saveData;
    private string _scoreFolder = "/SaveData/";
    private string _baseFileName = "saveDataFile.json";
    private string _fullFilePath;
    
    public void Initialize()
    {
        _saveData = new SaveData();
        string saveFolderPath = Application.persistentDataPath + _scoreFolder;
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath); 
        }
        _fullFilePath = Application.persistentDataPath + _scoreFolder + _baseFileName;
    }
    
    public void SaveGame(bool isGameReset)
    {
        _saveData = _saveDataContainer.GetData();
        
        // TODO: Save part of upgrades logic to be implemented here
        if (isGameReset)
        {
            _saveData.Wave = 0;
            _saveData.CurrentScore = 0;
            _saveData.Level = 0;
            _saveData.UpgradePoints = 0;
            _saveData.UpgradePointsThreshold = 5;
            _saveData.UpgradeThesholdIncrement = 5;
            _saveData.MaxHealth = 8;
            _saveData.Health = 8;
            _saveData.CooldownTime = 3f;
        }
        string json = JsonUtility.ToJson(_saveData);
        File.WriteAllText(_fullFilePath, json);
    }
   
    public void LoadGame()
    {
        if (File.Exists(_fullFilePath))
        {
            string json = File.ReadAllText(_fullFilePath);
            _saveData = JsonUtility.FromJson<SaveData>(json);
            _saveDataContainer.SetData(_saveData);
        }
        else
        {
            // TODO: Refactor this. We need the logical way to store default values <------------------------
            _saveData.Wave = 0;
            _saveData.CurrentScore = 0;
            _saveData.Level = 0;
            _saveData.UpgradePoints = 0;
            _saveData.UpgradePointsThreshold = 5;
            _saveData.UpgradeThesholdIncrement = 5;
            _saveData.MaxHealth = 8;
            _saveData.Health = 8;
            _saveData.CooldownTime = 3f;
            _saveDataContainer.SetData(_saveData);
        }
    }
}
