using System;
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
    [SerializeField] private FeedManager feedManager;
    [SerializeField] private UIManager uiManager;

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
    public void Feed(string label)
    {
        if(Enum.TryParse(label, out FoodType food))
        {
            uiManager.UpdateDrawingResult($"{name} ate {label}");
            feedManager.Feed(food);
        }
        else
        {
            Debug.Log($"Failed: {label}");
        }
    }
    public void Sleep(String label)
    {
        if(label == "Bed")
        {
            uiManager.UpdateDrawingResult($"{name} is Sleeping!");
            sleepManager.StartSleep();

        }
        else
        {
            Debug.Log($"Failed: {label}");
        }
    }
    public void UnkownDrawing()
    {
        uiManager.UpdateDrawingResult("What's that?! Ewwwww....");
    }

    public void OnDeath()
    {
        currentState = GameState.Dead;
        SceneManager.LoadScene("Death");
        Debug.Log("GameOver");
    }

    //Test
    [ContextMenu("Test Feed Vegetable")] void TestVegetable() => Feed("Vegetable");
    [ContextMenu("Test Feed Meat")] void TestMeat() => Feed("Meat");
    [ContextMenu("Test Feed Noodle")] void TestNoodle() => Feed("Noodle");
    [ContextMenu("Test Feed Bread")] void TestBread() => Feed("Bread");
    [ContextMenu("Test Feed Drink")] void TestDrink() => Feed("Drink");
    [ContextMenu("Test Sleep")] void TestSleep () => Sleep("Bed");

}
