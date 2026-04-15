using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField]
    private StatsManager statsManager;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private CreatureManager creatureManager;

    private float hungerDeathTimer = 0f;
    private float happinessDeathTimer = 0f;

    private bool isDeath = false;
    private const float DEATH_THREASHOLD_TIME = 10f;
    private const float WARNING_THREASHOLD_TIME = DEATH_THREASHOLD_TIME/2;

    private bool hungerWarningShown = false;
    private bool happinessWarningShown = false;

    private void Update()
    {
        if (isDeath) return;

        // hunger Timer
        if (statsManager.Hunger >= 100f)
        {
            hungerDeathTimer += Time.deltaTime;

            if(hungerDeathTimer >= WARNING_THREASHOLD_TIME && !hungerWarningShown)
            {
                float timeLeft = DEATH_THREASHOLD_TIME - hungerDeathTimer;
                uiManager.UpdateDrawingResult($"Starving!!!! Feed me or I'll die in {timeLeft:F0}s!");
                hungerWarningShown = true;
            }
        }
        else
        {
            hungerDeathTimer = 0f;
            hungerWarningShown= false;
        }

        // happiness Timer
        if (statsManager.Happiness <= 0.1f)
        {
            happinessDeathTimer += Time.deltaTime;

            if (happinessDeathTimer >= WARNING_THREASHOLD_TIME && !happinessWarningShown)
            {
                float timeLeft = DEATH_THREASHOLD_TIME - happinessDeathTimer;
                uiManager.UpdateDrawingResult($"Too Sad!!!! I'll die in {timeLeft:F0}s!");
                happinessWarningShown = true;
            }
        }
        else
        {
            happinessDeathTimer = 0f;
            happinessWarningShown = false;
        }

        float maxTimer = Mathf.Max(hungerDeathTimer, happinessDeathTimer);
        if(maxTimer >= WARNING_THREASHOLD_TIME)
        {
            float t = (maxTimer - WARNING_THREASHOLD_TIME) / (DEATH_THREASHOLD_TIME - WARNING_THREASHOLD_TIME);
            float alpha = Mathf.Lerp(1f, 0.3f, t);
            creatureManager.SetCreatureFade(alpha);
        }
        else
        {
            creatureManager.ResetFade();
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
