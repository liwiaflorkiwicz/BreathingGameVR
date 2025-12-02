using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections.Generic;
using TMPro;

public class GameScript : MonoBehaviour
{
    [SerializeField] private SliderInteraction sliders;
    [SerializeField] private Button ButtonStart;
    [SerializeField] private GameObject EndGamePanel;

    [SerializeField] private Transform LeftHandTransform;
    [SerializeField] private Transform RightHandTransform;

    [Header("UI Elements & Bubble")]
    [SerializeField] private GameObject CountdownPanel;
    [SerializeField] private TextMeshProUGUI CountdownText;
    [SerializeField] private Transform BubbleTransform;

    [SerializeField] private Vector3 minBubbleScale = new Vector3(50f, 50f, 50f);
    [SerializeField] private Vector3 maxBubbleScale = new Vector3(200f, 200f, 200f);

    void Start()
    {
        ButtonStart.onClick.RemoveAllListeners();
        ButtonStart.onClick.AddListener(StartGameButtonPressed);

        if (EndGamePanel != null) EndGamePanel.SetActive(false);
        if (CountdownPanel != null) CountdownPanel.SetActive(false);
        if (BubbleTransform != null) BubbleTransform.gameObject.SetActive(false);
    }

    public void StartGameButtonPressed()
    {
        int repsInt = Mathf.RoundToInt(sliders.Reps);
        DataSender.SendSliderData(sliders.Inhale, sliders.Exhale, repsInt);

        StartCoroutine(GameCoroutine());
    }

    private IEnumerator GameCoroutine()
    {
        // 1. COUNTDOWN 
        if (CountdownPanel != null && CountdownText != null)
        {
            CountdownPanel.SetActive(true);
            for (int i = 5; i > 0; i--)
            {
                CountdownText.text = i.ToString();
                yield return new WaitForSeconds(1.0f);
            }
            CountdownPanel.SetActive(false);
        }

        // 2. GAME
        if (BubbleTransform != null) BubbleTransform.gameObject.SetActive(true);

        float inhaleTime = sliders.Inhale;
        float exhaleTime = sliders.Exhale;
        float totalReps = sliders.Reps;
        float holdDuration = 0.2f;

        float cycleDuration = inhaleTime + holdDuration + exhaleTime + holdDuration;
        float totalDuration = totalReps * cycleDuration;

        float elapsed = 0f;
        float sampleTimer = 0f;
        float sampleInterval = 0.1f;

        yield return new WaitForSeconds(0.5f);

        float minHeight = 0.0f; 
        float maxHeight = 2.6f; 

        while (elapsed < totalDuration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            sampleTimer += dt;

            float currentCycleTime = elapsed % cycleDuration;
            float endOfInhale = inhaleTime;
            float endOfHoldTop = inhaleTime + holdDuration;
            float endOfExhale = inhaleTime + holdDuration + exhaleTime;

            if (currentCycleTime < endOfInhale)
                BubbleTransform.localScale = Vector3.Lerp(minBubbleScale, maxBubbleScale, currentCycleTime / inhaleTime);
            else if (currentCycleTime < endOfHoldTop)
                BubbleTransform.localScale = maxBubbleScale;
            else if (currentCycleTime < endOfExhale)
                BubbleTransform.localScale = Vector3.Lerp(maxBubbleScale, minBubbleScale, (currentCycleTime - endOfHoldTop) / exhaleTime);
            else
                BubbleTransform.localScale = minBubbleScale;

            if (sampleTimer >= sampleInterval)
            {
                sampleTimer = 0f;

                // Getting raw position
                float rawLeftY = LeftHandTransform.position.y;
                float rawRightY = RightHandTransform.position.y;

                Debug.Log($"[RAW] LeftY: {rawLeftY:F3} | RightY: {rawRightY:F3}");

                float leftY = Mathf.Clamp01((rawLeftY - minHeight) / (maxHeight - minHeight));
                float rightY = Mathf.Clamp01((rawRightY - minHeight) / (maxHeight - minHeight));

                DataSender.AddControllerSample(elapsed, leftY, rightY);
            }

            yield return null;
        }

        if (BubbleTransform != null) BubbleTransform.gameObject.SetActive(false);

        Debug.Log("Game ended. Sending data...");

        bool uploadFinished = false;
        DataSender.SendControllersData((success) => { uploadFinished = true; });

        while (!uploadFinished) yield return null;

        if (EndGamePanel != null) EndGamePanel.SetActive(true);
    }
}