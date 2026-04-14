using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TimeManager timeManager;
    [SerializeField]
    private StatsManager statsManager;
    [SerializeField]
    private FeedManager feedManager;
    [SerializeField]
    private GameManager gameManager;

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
    private TMP_Text likeText;
    [SerializeField]
    private TMP_Text dislikeText;
    [SerializeField]
    private TMP_Text neutralText;

    [SerializeField]
    private TMP_Text drawingExplainText;

    [SerializeField]
    private GameObject statsPopup;

    private float subtitleTimer = 0f;
    private float subtitleDuration = 5f;

    private void Start()
    {
        UpdatePreference();
    }
    // Update is called once per frame
    void Update()
    {
        dateText.text = $"{timeManager.CurrentMonth}/{timeManager.CurrentDay}";
        timeText.text = $"{timeManager.CurrentHour:D2} : {timeManager.CurrentMinute:D2}";

        hungerText.text = $"Hunger: {statsManager.Hunger:F1}";
        happinessText.text = $"Happiness: {statsManager.Happiness:F1}";
        energyText.text = $"Energy: {statsManager.Energy:F1}";

        if(subtitleTimer > 0f)
        {
            subtitleTimer -= Time.deltaTime;
            if(subtitleTimer <= 0f)
            {
                drawingExplainText.text = "";
            }
        }
    }

    public void ToggleStatsPopup()
    {
        statsPopup.SetActive(!statsPopup.activeSelf);
    }

    public void UpdateDrawingResult(string explain)
    {
        drawingExplainText.text = explain;
        subtitleTimer = subtitleDuration;
    }

    public void UpdatePreference()
    {
        if (gameManager.State != GameState.Adult)
        {
            string preferenceText = "Neutral: ";

            for (int i = 0; i < 5; i++) {
                preferenceText = preferenceText + feedManager.Foods[i] + "  ";
            }
            neutralText.text = preferenceText;
        }
        else
        {
            likeText.text = $"Like: {feedManager.Foods[0]}, {feedManager.Foods[1]}";
            dislikeText.text = $"Dislike: {feedManager.Foods[2]}, {feedManager.Foods[3]}";
            neutralText.text = $"Neutral: {feedManager.Foods[4]}";
        }
    }

    public void ShowGrowthMSG(GameState newState, string name)
    {
        string msg = newState switch
        {
            GameState.Baby => $"Your {name} hatched!",
            GameState.Adult => $"Your {name} grew up!",
            _ => ""
        };

        if (!string.IsNullOrEmpty(msg)) { 
            UpdateDrawingResult(msg);
        }

    }

    public void ShowPreferenceUnlockedMSG()
    {
        UpdateDrawingResult("Food Preferences unlocked!");
    }
}
