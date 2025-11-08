using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class DataSender : MonoBehaviour
{
    private static string serverUrl = "http://127.0.0.1:5000/api/data"; // tu wpiszesz później swój adres

    [System.Serializable]
    public class GameData
    {
        public float inhale;
        public float exhale;
        public float reps;
        public List<ControllerFrame> controllersData = new List<ControllerFrame>();
    }

    [System.Serializable]
    public class ControllerFrame
    {
        public float time;
        public float leftY;
        public float rightY;
    }

    public static List<ControllerFrame> controllerFrames = new List<ControllerFrame>();

    public static void AddControllerSample(float time, float leftY, float rightY)
    {
        controllerFrames.Add(new ControllerFrame
        {
            time = time,
            leftY = leftY,
            rightY = rightY
        });
    }

    public static GameData data;

    public static void SendSliderData(float inhale, float exhale, float reps)
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
            sender.StartCoroutine(sender.SendRequest(json));
    }

    public static int GetSampleCount()
    {
        return controllerFrames.Count;
    }


    public static void SendControllersData()
    {
        if (controllerFrames.Count == 0)
        {
            Debug.LogWarning("No controller samples collected.");
            return;
        }

        data.controllersData = new List<ControllerFrame>(controllerFrames);
        
        string json = JsonUtility.ToJson(data);
        Debug.Log("Sending controllers JSON: " + json);

        DataSender sender = GameObject.FindFirstObjectByType<DataSender>();
        if (sender != null)
            sender.StartCoroutine(sender.SendRequest(json));

        controllerFrames.Clear();
    }

    private IEnumerator SendRequest(string json)
    {
        using (UnityWebRequest www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                Debug.Log("Data sent correctly");
            else
                Debug.LogError("Sending error: " + www.error);
        }
    }
}
