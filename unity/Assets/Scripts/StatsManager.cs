using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField]
    private TimeManager timeManager;
    [SerializeField]
    private SleepManager sleepManager;
    [SerializeField]
    private GameManager gameManager;

    private float hunger = 0f;
    private float happiness = 100f;
    private float energy = 100f;

    [SerializeField]
    private float hungerDecayPerSecond = 2.5f / 60f;
    [SerializeField]
    private float happinessDecayPerSecond = 2.5f / 60f;
    [SerializeField]
    private float energyDecayPerSecond = 2.5f / 60f;
    [SerializeField]
    private float energyNightDecayPerSecond = 12f / 60f;


    // Getter
    public float Hunger => hunger;
    public float Happiness => happiness;
    public float Energy => energy;
    


    // Update is called once per frame
    void Update()
    {
        float currentHungryDecay = timeManager.IsMealTime ?
            hungerDecayPerSecond * 2f : hungerDecayPerSecond;

        hunger += currentHungryDecay * Time.deltaTime;

        float happinessDecay = happinessDecayPerSecond;
        if (hunger >= 99.99f) {
            happinessDecay *= 2f;
        }

        if (sleepManager.IsSleeping)
        {
            happiness -= happinessDecayPerSecond * 0.5f * Time.deltaTime;
        }
        else
        {
            happiness -= happinessDecayPerSecond * Time.deltaTime;
        }
            
        

        if (timeManager.IsNight)
        {
            energy -= energyNightDecayPerSecond * Time.deltaTime;
        }
        else
        {
            energy -= energyDecayPerSecond * Time.deltaTime;
        }

        hunger = Mathf.Clamp(hunger, 0, 100);
        happiness = Mathf.Clamp(happiness, 0, 100);
        energy = Mathf.Clamp(energy, -100, 100);

        StatsWarning();
        CheckGrowth();


    }

    private void CheckGrowth()
    {
        if (gameManager.State != GameState.Baby) return;
        if (timeManager.GameDay < 2) return;

        float avg = (100f - hunger + happiness) / 2f;
        if(avg > 50f)
        {
            gameManager.OnGrowToAdult();
        }
    }

    public void ResetStats()
    {
        hunger = 0f;
        happiness = 100f;
        energy = 100f;
    }

    public void StatsWarning()
    {
        if (hunger >= 70f)
            Debug.LogWarning("Hunger Warning!");
        if (happiness <= 30f)
            Debug.LogWarning("Happiness Warning!");
        if (energy <= 30f)
            Debug.LogWarning("Energy Warning!");
    }
    public void OnWakeUp()
    {
        energy = 100f;
        happiness = Mathf.Clamp(happiness + 30f, 0f, 100f);
    }

    public void ApplyFeed(float hungerAmount, float happinessAmount)
    {
        hunger = Mathf.Clamp(hunger - hungerAmount, 0f, 100f);
        happiness = Mathf.Clamp(happiness + happinessAmount, 0f, 100f);
    }

    public void ApplyOverFeed()
    {
        energy = Mathf.Clamp(energy - 10f, -100f, 100f);
        happiness = Mathf.Clamp(happiness - 10f, 0f, 100f);
    }

    //public void Feed()
    //{
    //    hunger = Mathf.Clamp(hunger - 20f, 0f, 100f);
    //    happiness = Mathf.Clamp(happiness + 10f, 0f, 100f);
    //    Debug.Log("Fed");
    //}

}
