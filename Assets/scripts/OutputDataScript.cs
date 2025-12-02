using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class OutputDataScript : MonoBehaviour
{
    private static string serverUrl = "http://192.168.43.165:5000/api/data/results";

    [SerializeField] private TextMeshProUGUI ResultsText;
    [SerializeField] private GraphRenderer graphRenderer;

    void OnEnable()
    {
        StartCoroutine(GetDataFromServer());
    }

    private IEnumerator GetDataFromServer()
    {
        string mySessionId = DataSender.currentSessionId;

        if (string.IsNullOrEmpty(mySessionId))
        {
            Debug.LogWarning("NO SessionID.");
            if (ResultsText != null) ResultsText.text = "No session data.";
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        string finalUrl = serverUrl + "/" + mySessionId;
        Debug.Log("Sending GET to: " + finalUrl);

        using (UnityWebRequest www = UnityWebRequest.Get(finalUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("Received data from server: " + json);

                DataSender.ResultSummary summary = JsonUtility.FromJson<DataSender.ResultSummary>(json);

                if (graphRenderer != null && summary.graphData != null)
                {
                    graphRenderer.DrawGraph(summary.graphData, summary.overallAccuracy);
                }
            }
            else
            {
                Debug.LogError("Error fetching data: " + www.error);
                if (ResultsText != null) ResultsText.text = "Error loading results.";
            }
        }
    }
}