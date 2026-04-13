using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Egg,
    Hatching,
    Baby,
    Adult,
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

    private GameState currentState = GameState.Egg;

    //Getter
    public GameState State => currentState;

    public void StartGame(int month, int day)
    {
        currentState = GameState.Egg;
        timeManager.StartTime(month, day);
        timeManager.SetPaused(true);
        Debug.Log($"Game Start {month}/{day}");
        Invoke("OhHatched", 3f);
    }
    public void OhHatched()
    {
        currentState = GameState.Hatching;
        Invoke("ToBaby", 3f);
        Debug.Log("Hatching stage");
    }

    public void ToBaby()
    {
        currentState = GameState.Baby;
        timeManager.SetPaused(false);
        timeManager.ResetGameDay();
        Debug.Log("Baby stage");
    }

    public void OnGrowToAdult()
    {
        currentState=GameState.Adult;
        Debug.Log("Adult stage");
    }

    public void RestartGame()
    {
        statsManager.ResetStats();
        timeManager.ResetGameDay();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Start");
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
