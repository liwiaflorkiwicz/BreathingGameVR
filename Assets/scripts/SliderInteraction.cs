using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInteraction : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider sliderInhale;
    [SerializeField] private Slider sliderExhale;
    [SerializeField] private Slider sliderReps;

    [SerializeField] private TextMeshProUGUI inhaleValue;
    [SerializeField] private TextMeshProUGUI exhaleValue;
    [SerializeField] private TextMeshProUGUI repsValue;

    void Start()
    {
        sliderInhale.onValueChanged.AddListener((value) =>
        {
            inhaleValue.text = value.ToString("0.0"); // format recommended for decimals
        });

        sliderExhale.onValueChanged.AddListener((value) =>
        {
            exhaleValue.text = value.ToString("0.0");
        });

        sliderReps.onValueChanged.AddListener((value) =>
        {
            repsValue.text = value.ToString();
        });

        // Initialize text values on startup
        Update();
    }

    // Public properties to be accessed by the GameScript.
    // These values control breath duration and mechanics (e.g., soap bubble expansion).
    public float Inhale => sliderInhale.value;
    public float Exhale => sliderExhale.value;
    public float Reps => sliderReps.value;

    void Update()
    {

    }
}