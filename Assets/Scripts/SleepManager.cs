using UnityEngine;

public class SleepManager : MonoBehaviour
{
    [SerializeField]
    private StatsManager statsManager;
    [SerializeField]
    private TimeManager timeManager;

    private bool isSleeping = false;
    
    // Getter
    public bool IsSleeping => isSleeping;

    // Update is called once per frame
    void Update()
    {
        if (!isSleeping)
        {
            if(statsManager.Energy <= -100f)
            {
                StartSleep();
            }
        }

        if (isSleeping)
        {
            if(timeManager.CurrentHour == 7 && timeManager.CurrentMinute == 0)
            {
                WakeUp();
            }
        }
    }
    public void StartSleep()
    {
        isSleeping = true;
        Debug.Log("Sleep start");
    }

    public void WakeUp()
    {
        isSleeping = false;
        Debug.Log("Wake up");
    }
}
