using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class ControllerFrame
{
    public float time;    // czas od początku gry
    public float leftY;
    public float rightY;
}

[System.Serializable]
public class GameSessionData
{
    public float inhale;
    public float exhale;
    public int reps;
    public ControllerFrame[] controllersData;
}

[System.Serializable]
public class ResultSummary
{
    public float overallAccuracy;
    public string summaryTable;
    public string sessionId;
}

public class OutputDataScript : MonoBehaviour
{
    private static string serverUrl = "http://127.0.0.1:5000/api/data/results";
    [SerializeField] private TextMeshProUGUI ResultsText;

    void Start()
    {
        StartCoroutine(GetDataFromServer());
    }

    public class SessionWrapper
    {
        public GameSessionData session;
    }

    private IEnumerator GetDataFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(serverUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("Received data from server: " + json);

                ResultSummary summary = JsonUtility.FromJson<ResultSummary>(json);

                if (ResultsText != null)
                    ResultsText.text = summary.summaryTable;
            }
            else
            {
                Debug.LogError("Error fetching data: " + www.error);
            }
        }
    }
}
