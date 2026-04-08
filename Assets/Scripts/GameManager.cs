using UnityEngine;

public enum GameState
{
    Egg,
    Playing,
    Sleeping,
    Dead
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private SleepManager sleepManager;
    [SerializeField] private DeathManager deathManager;

    public GameState currentState = GameState.Egg;

    public void StartGame(int month, int day)
    {
        currentState = GameState.Playing;
        timeManager.StartTime(month, day);
        Debug.Log($"Game Start {month}/{day}");
    }

    public void RestartGame()
    {
        currentState = GameState.Egg;
        statsManager.ResetStats();
        Debug.Log("Game Reset");
    }

    private void Start()
    {
        string name = PlayerPrefs.GetString("name");
        int month = PlayerPrefs.GetInt("month");
        int day = PlayerPrefs.GetInt("day");

        Debug.Log($"Creature: {name}, {month}/{day}");
        StartGame(month, day);

    }

}
