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

public class OutputDataScript : MonoBehaviour
{
    private static string serverUrl = "http://127.0.0.1:5000/api/data";
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

                GameSessionData session = JsonUtility.FromJson<GameSessionData>(json);

                if (ResultsText != null)
                    ResultsText.text = GeneratePerRepSummary(session);
            }
            else
            {
                Debug.LogError("Error fetching data: " + www.error);
            }
        }
    }

    private string GeneratePerRepSummary(GameSessionData session)
    {
        if (session.controllersData == null || session.controllersData.Length == 0)
            return "No controller data";

        string output = "Rep\tPhase\tSliderTime(s)\tLeftHand\tRightHand\n";

        float inhaleTime = session.inhale;
        float exhaleTime = session.exhale;
        int reps = session.reps;

        int frameIndex = 0;

        for (int r = 0; r < reps; r++)
        {
            // INHALE
            float inhaleStart = r * (inhaleTime + exhaleTime);
            float inhaleEnd = inhaleStart + inhaleTime;
            bool leftMatchInhale = false;
            bool rightMatchInhale = false;

            while (frameIndex < session.controllersData.Length &&
                   session.controllersData[frameIndex].time <= inhaleEnd)
            {
                var frame = session.controllersData[frameIndex];
                if (frame.leftY > 0.5f) leftMatchInhale = true;
                if (frame.rightY > 0.5f) rightMatchInhale = true;
                frameIndex++;
            }

            output += $"{r + 1}\tInhale\t{inhaleTime:F2}\t{(leftMatchInhale ? "V" : "X")}\t{(rightMatchInhale ? "V" : "X")}\n";

            // EXHALE
            float exhaleStart = inhaleEnd;
            float exhaleEnd = exhaleStart + exhaleTime;
            bool leftMatchExhale = false;
            bool rightMatchExhale = false;

            while (frameIndex < session.controllersData.Length &&
                   session.controllersData[frameIndex].time <= exhaleEnd)
            {
                var frame = session.controllersData[frameIndex];
                if (frame.leftY < 0.5f) leftMatchExhale = true;
                if (frame.rightY < 0.5f) rightMatchExhale = true;
                frameIndex++;
            }

            output += $"{r + 1}\tExhale\t{exhaleTime:F2}\t{(leftMatchExhale ? "V" : "X")}\t{(rightMatchExhale ? "V" : "X")}\n";
        }

        // Opcjonalnie: podsumowanie procentowe
        int totalChecks = reps * 2 * 2; // inhale+exhale * left+right
        int totalCorrect = 0;
        foreach (var c in output)
        {
            if (c == 'V') totalCorrect++;
        }

        float percent = (float)totalCorrect / totalChecks * 100f;
        output += $"\nOverall Accuracy: {percent:F1}%";

        return output;
    }
}
