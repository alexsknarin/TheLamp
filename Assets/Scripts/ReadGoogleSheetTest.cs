using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class ReadGoogleSheetTest : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadSheetData());
    }


    IEnumerator LoadSheetData()
    {
        UnityWebRequest www = UnityWebRequest.Get("_");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result== UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Connection ERROR: " + www.error);
        }
        else
        {
            string jsonData = www.downloadHandler.text;
            var jsonObject = JSON.Parse(jsonData);
            int i = 0;
            foreach (var item in jsonObject["values"])
            {
                var itemJsonObject = JSON.Parse(item.ToString());
                if (i > 0)
                {
                    Debug.Log("Fly: " + itemJsonObject[0][1].ToString().Replace("\"", "") +
                                  " Moth: " + itemJsonObject[0][2].ToString().Replace("\"", "") + 
                                 " Firefly: " + itemJsonObject[0][3].ToString().Replace("\"", ""));
                    
                    
                }
                i++;
            }
        }
    }

}
