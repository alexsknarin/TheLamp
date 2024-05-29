using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour, IInitializable
{
    [SerializeField] private SaveDataContainer _saveDataContainer;
    [SerializeField] private DefaultStatsContainer _defaultStatsContainer;
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
    
    public void SaveTempData()
    {
        // Only keep upgrades in memory
        _saveData = _saveDataContainer.GetData();
        _saveDataContainer.SetData(_defaultStatsContainer.GetData());
        string json = JsonUtility.ToJson(_defaultStatsContainer.GetData());
        File.WriteAllText(_fullFilePath, json);
    }
    
    public void SaveGame(bool isGameReset, bool isSaveUpgrades)
    {
        if (isGameReset)
        {
            if (isSaveUpgrades)
            {
                _saveData = _defaultStatsContainer.GetDataKeepUpgrades(_saveData);
                _saveDataContainer.SetData(_saveData);    
            }
            else
            {
                _saveData = _defaultStatsContainer.GetData();
                _saveDataContainer.SetData(_saveData);
            }
        }
        else
        {
            _saveData = _saveDataContainer.GetData();    
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
            _saveData = _defaultStatsContainer.GetData();
            _saveDataContainer.SetData(_saveData);
        }
    }
}
