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
        dateText.text = $"{timeManager.CurrentMonth}/{timeManager.CurrentDay}";
        timeText.text = $"{timeManager.CurrentHour:D2} : {timeManager.CurrentMinute:D2}";
    }
}
