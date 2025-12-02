using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DataSender : MonoBehaviour
{
    private static string serverUrl = "http://192.168.43.165:5000/api/data";
    public static string currentSessionId = "";
    public static List<ControllerFrame> controllerFrames = new List<ControllerFrame>();
    public static GameData data;

    // --- DATA CLASSES (Defined only here) ---

    [System.Serializable]
    public class SessionResponse
    {
        public string status;
        public string message;
        public string sessionId;
    }

    [System.Serializable]
    public class GameData
    {
        public float inhale;
        public float exhale;
        public int reps;
        public List<ControllerFrame> controllersData = new List<ControllerFrame>();
    }

    [System.Serializable]
    public class ControllerFrame
    {
        public float time;
        public float leftY;
        public float rightY;
    }

    // Adding ResultSummary class here so OutputDataScript can use it
    [System.Serializable]
    public class ResultSummary
    {
        public float overallAccuracy;
        public List<ControllerFrame> graphData; // Uses DataSender.ControllerFrame
        public string sessionId;
    }

    // --- SENDING LOGIC ---

    public static void AddControllerSample(float time, float leftY, float rightY)
    {
        controllerFrames.Add(new ControllerFrame
        {
            time = time,
            leftY = leftY,
            rightY = rightY
        });
    }

    public static void SendSliderData(float inhale, float exhale, int reps)
    {
        data = new GameData
        {
            inhale = inhale,
            exhale = exhale,
            reps = reps,
            controllersData = null
        };

        controllerFrames.Clear();
        string json = JsonUtility.ToJson(data);
        Debug.Log("Sending sliders JSON: " + json);

        DataSender sender = GameObject.FindFirstObjectByType<DataSender>();
        if (sender != null)
            sender.StartCoroutine(sender.SendPreSessionRequest(json));
    }

    private IEnumerator SendPreSessionRequest(string json)
    {
        string url = serverUrl + "/presession";
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                SessionResponse response = JsonUtility.FromJson<SessionResponse>(www.downloadHandler.text);
                currentSessionId = response.sessionId;
                Debug.Log("Pre-session data sent. Received SessionID: " + currentSessionId);
            }
            else
            {
                Debug.LogError("Error POST /presession: " + www.error);
            }
        }
    }

    public static int GetSampleCount()
    {
        return controllerFrames.Count;
    }

    public static void SendControllersData(System.Action<bool> onComplete = null)
    {
        if (controllerFrames.Count == 0)
        {
            Debug.LogWarning("No controller samples collected.");
            onComplete?.Invoke(false);
            return;
        }

        data.controllersData = new List<ControllerFrame>(controllerFrames);

        string json = JsonUtility.ToJson(data);
        // Debug.Log("Sending controllers JSON: " + json);

        DataSender sender = GameObject.FindFirstObjectByType<DataSender>();
        if (sender != null)
            sender.StartCoroutine(sender.SendPostSessionRequest(json, onComplete));
        else
            onComplete?.Invoke(false);

        controllerFrames.Clear();
    }

    private IEnumerator SendPostSessionRequest(string json, System.Action<bool> onComplete)
    {
        string url = serverUrl + "/postsession/" + currentSessionId;

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Controllers data sent successfully.");
                onComplete?.Invoke(true); // Success
            }
            else
            {
                Debug.LogError("Sending error: " + www.error);
                onComplete?.Invoke(false); // Error
            }
        }
    }
}