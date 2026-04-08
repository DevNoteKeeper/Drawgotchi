using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TimeManager timeManager;
    [SerializeField]
    private StatsManager statsManager;

    [SerializeField]
    private TMP_Text dateText;
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text hungerText;
    [SerializeField]
    private TMP_Text happinessText;
    [SerializeField]
    private TMP_Text energyText;

    [SerializeField]
    private GameObject statsPopup;


    // Update is called once per frame
    void Update()
    {
        dateText.text = $"{timeManager.CurrentMonth}/{timeManager.CurrentDay}";
        timeText.text = $"{timeManager.CurrentHour:D2} : {timeManager.CurrentMinute:D2}";

        hungerText.text = $"Hunger: {statsManager.Hunger:F1}";
        happinessText.text = $"Happiness: {statsManager.Happiness:F1}";
        energyText.text = $"Energy: {statsManager.Energy:F1}";

    }

    public void ToggleStatsPopup()
    {
        statsPopup.SetActive(!statsPopup.activeSelf);
    }
}
