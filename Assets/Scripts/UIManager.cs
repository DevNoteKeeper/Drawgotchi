using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TimeManager timeManager;
    [SerializeField]
    private TMP_Text dateText;
    [SerializeField]
    private TMP_Text timeText;


    // Update is called once per frame
    void Update()
    {
        dateText.text = $"{timeManager.currentMonth}/{timeManager.currentDay}";
        timeText.text = $"{timeManager.currentHour:D2} : {timeManager.currentMinute:D2}";
    }
}
