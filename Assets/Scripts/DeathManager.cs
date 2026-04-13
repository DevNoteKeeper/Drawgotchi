using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField]
    private StatsManager statsManager;
    [SerializeField]
    private GameManager gameManager;

    private float hungerDeathTimer = 0f;
    private float happinessDeathTimer = 0f;
    private bool isDeath = false;
    private const float DEATH_THREASHOLD_TIME = 60f;

    private void Update()
    {
        if (isDeath) return;

        // hunger Timer
        if (statsManager.Hunger >= 100f)
        {
            hungerDeathTimer += Time.deltaTime;
        }
        else
        {
            hungerDeathTimer = 0f;
        }

        // happiness Timer
        if (statsManager.Happiness <= 0.1f)
        {
            happinessDeathTimer += Time.deltaTime;
        }
        else
        {
            happinessDeathTimer = 0f;
        }

        // whether check death
        if (hungerDeathTimer >= DEATH_THREASHOLD_TIME || happinessDeathTimer >= DEATH_THREASHOLD_TIME)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        isDeath = true;
        gameManager.OnDeath();
        Debug.Log("Creature died");
    }
}
