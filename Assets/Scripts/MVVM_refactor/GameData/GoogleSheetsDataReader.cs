using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "GoogleSheetsDataReader", menuName = "GoogleSheetsDataReader")]
public class GoogleSheetsDataReader : ScriptableObject, IInitializable
{
    [SerializeField] private string _sheetId;
    [SerializeField] private string _sheetName;
    [SerializeField] private string _apiKey;
    [SerializeField] private bool _useCachedSpawnData;
    [SerializeField] private SpawnQueueData _spawnQueueDataCache;
    private string _sheetData;
    private CoroutineHost _coroutineHost;

    public void Construct(CoroutineHost coroutineHost)
    {
        _coroutineHost = coroutineHost;
    }
    
    public event Action OnDataLoadedEvent;
    public string SheetData => _sheetData;

    public void Initialize()
    {
        if (_useCachedSpawnData)
        {
            OnDataLoadedEvent?.Invoke();
        }
        else
        {
            _coroutineHost.StartCoroutine(LoadSheetData());    
        }
    }
    
    IEnumerator LoadSheetData()
    {
        string url = "https://sheets.googleapis.com/v4/spreadsheets/" + _sheetId + "/values/" + _sheetName + "?key=" + _apiKey;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result== UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Connection ERROR: " + www.error);
            _sheetData = "";
            OnDataLoadedEvent?.Invoke();
        }
        else
        {
            _sheetData = www.downloadHandler.text;
            _spawnQueueDataCache.Data = _sheetData;
            OnDataLoadedEvent?.Invoke();
        }
    }
}
