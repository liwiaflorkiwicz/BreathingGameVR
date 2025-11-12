using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{

    // Napisac skrypt gry co mozna zrobic while start gry

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private SliderInteraction sliders;
    [SerializeField] private Button ButtonStart;
    [SerializeField] private GameObject EndGamePanel;

    void Start()
    {
        ButtonStart.onClick.RemoveAllListeners();
        ButtonStart.onClick.AddListener(StartGameButtonPressed);
        if (EndGamePanel != null)
            EndGamePanel.SetActive(false);
    }

    public void StartGameButtonPressed()
    {
        // Wyœlij dane ze sliderów natychmiast
        int repsInt = Mathf.RoundToInt(sliders.Reps);
        DataSender.SendSliderData(sliders.Inhale, sliders.Exhale, repsInt);

        // Rozpocznij grê (odliczanie 5 sekund)
        StartCoroutine(GameCoroutine());
    }

    private IEnumerator GameCoroutine()
    {
        // Ca³kowity czas gry = reps * (inhale + exhale)
        float totalDuration = sliders.Reps * (sliders.Inhale + sliders.Exhale);
        float elapsed = 0f;
        float sampleInterval = 0.5f;

        Debug.Log($"Game started. Duration: {totalDuration}s");

        // Czekaj krótk¹ chwilê, aby mieæ pewnoœæ, ¿e SendSliderData siê wykona³
        yield return new WaitForSeconds(0.5f);

        while (elapsed < totalDuration)
        {
            float leftY = Random.Range(0f, 1f);
            float rightY = Random.Range(0f, 1f);

            DataSender.AddControllerSample(elapsed, leftY, rightY);
            Debug.Log($"[Sample] t={elapsed:F1}s L={leftY:F2} R={rightY:F2}");

            yield return new WaitForSeconds(sampleInterval);
            elapsed += sampleInterval;
        }

        Debug.Log($"Game ended. Sending {DataSender.GetSampleCount()} samples...");

        // Po zakoñczeniu gry – wyœlij dane kontrolerów
        DataSender.SendControllersData();
        yield return new WaitForSeconds(0.5f);

        if (EndGamePanel != null)
            EndGamePanel.SetActive(true);

    }

}

