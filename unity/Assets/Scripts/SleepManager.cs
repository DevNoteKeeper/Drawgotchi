using UnityEngine;

public class SleepManager : MonoBehaviour
{
    [SerializeField]
    private StatsManager statsManager;
    [SerializeField]
    private TimeManager timeManager;
    [SerializeField]
    private UIManager uIManager;

    private bool isSleeping = false;
    private float sleepTimer = 0f;
    private const float SLEEP_DURATION = 10f;

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
                return;
            }
        }

        sleepTimer += Time.deltaTime;
     
        if (sleepTimer >= SLEEP_DURATION && isSleeping)
        {
                WakeUp();

        }
    }
    public void StartSleep()
    {
        if (isSleeping)
        {
            return;
        }
        isSleeping = true;
        sleepTimer = 0f;
        Debug.Log("Sleep start");
    }

    public void WakeUp()
    {
        isSleeping = false;
        sleepTimer = 0f;
        timeManager.AdvancedToNextMorning();
        statsManager.OnWakeUp();
        uIManager.UpdateDrawingResult("Good Morning!");
        Debug.Log("Wake up");
    }

    public void TriggerSleep()
    {
        StartSleep();
        Debug.Log("Sleep trigger");
    }
}
