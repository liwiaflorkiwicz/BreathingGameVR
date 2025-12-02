using System.Collections.Generic;
using UnityEngine;
using TMPro; // If you want to display %

public class GraphRenderer : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Settings")]
    [SerializeField] private float width = 2.0f;  // Graph width in Unity units
    [SerializeField] private float height = 1.0f; // Graph height in Unity units

    // Optional text to display percentages
    [SerializeField] private TextMeshProUGUI accuracyText;

    public void DrawGraph(List<DataSender.ControllerFrame> data, float accuracy)
    {
        if (data == null || data.Count < 2) return;

        // Clear old lines
        leftLineRenderer.positionCount = data.Count;
        rightLineRenderer.positionCount = data.Count;

        leftLineRenderer.useWorldSpace = false;
        rightLineRenderer.useWorldSpace = false;

        // Find total time to scale the X axis
        float startTime = data[0].time;
        float totalTime = data[data.Count - 1].time - startTime;

        if (totalTime <= 0) totalTime = 1f; // Prevention against division by zero

        for (int i = 0; i < data.Count; i++)
        {
            float currentTime = data[i].time - startTime;

            // X normalization (time): from 0 to 'width'
            float xPos = (currentTime / totalTime) * width;

            // Y normalization (height): from 0 to 'height'
            float yPosLeft = data[i].leftY * height;
            float yPosRight = data[i].rightY * height;

            // Setting points (relative to this object's position)
            Vector3 pointLeft = new Vector3(xPos, yPosLeft, 0);
            Vector3 pointRight = new Vector3(xPos, yPosRight, 0);

            leftLineRenderer.SetPosition(i, pointLeft);
            rightLineRenderer.SetPosition(i, pointRight);
        }

        // Display percentage result if text is assigned
        if (accuracyText != null)
        {
            accuracyText.text = $"Accuracy: {accuracy:F1}%";
        }
    }
}