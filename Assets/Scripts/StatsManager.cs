using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField]
    private TimeManager timeManager;

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


    // Update is called once per frame
    void Update()
    {
        hunger += hungerDecayPerSecond * Time.deltaTime;
        happiness -= happinessDecayPerSecond * Time.deltaTime;
        

        if (timeManager.IsNight())
        {
            energy -= energyNightDecayPerSecond * Time.deltaTime;
        }
        else
        {
            energy -= energyDecayPerSecond * Time.deltaTime;
        }

        hunger = Mathf.Clamp(hunger, 0, 100);
        happiness = Mathf.Clamp(happiness, 0, 100);
        energy = Mathf.Clamp(energy, 0, 100);

        StatsWarning();
        Debug.Log(energy);
    }

    void StatsWarning()
    {
        if (hunger >= 70f)
            Debug.LogWarning("Hunger Warning!");
        if (happiness <= 30f)
            Debug.LogWarning("Happiness Warning!");
        if (energy <= 30f)
            Debug.LogWarning("Energy Warning!");
    }

}
