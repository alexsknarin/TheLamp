using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetsDataReader : MonoBehaviour, IInitializable
{
    [SerializeField] private string _sheetId;
    [SerializeField] private string _sheetName;
    [SerializeField] private string _apiKey;
    private string _sheetData;
    public string SheetData => _sheetData;
    
    public event Action OnDataLoaded; 

    public void Initialize()
    {
        StartCoroutine(LoadSheetData());
    }
    
    IEnumerator LoadSheetData()
    {
        string url = "https://sheets.googleapis.com/v4/spreadsheets/" + _sheetId + "/values/" + _sheetName + "?key=" + _apiKey;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result== UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Connection ERROR: " + www.error);
        }
        else
        {
            _sheetData = www.downloadHandler.text;
            OnDataLoaded?.Invoke();
        }
    }
}
