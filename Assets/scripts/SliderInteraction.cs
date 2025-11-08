using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderInteraction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
            inhaleValue.text = value.ToString();
        });

        sliderExhale.onValueChanged.AddListener((value) =>
        {
            exhaleValue.text = value.ToString();
        });

        sliderReps.onValueChanged.AddListener((value) =>
        {
            repsValue.text = value.ToString();
        });

    }

    // Save input values to pass to GameScript
    // Te wartosci wykorzystac jakos jako dlugosci oddechow (inhale & exhale) jako
    // np. powiekszanie sie banki mydlanej

    // Update is called once per frame
    void Update()
    {
        
    }
}
