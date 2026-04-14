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

    private string creatureName;

    private GameState currentState = GameState.Egg;

    //Getter
    public GameState State => currentState;

    public void StartGame(string name, int month, int day)
    {
        currentState = GameState.Egg;
        
        timeManager.StartTime(month, day);
        timeManager.SetPaused(true);
        creatureName = name;
        uiManager.UpdateCreatueState(GameState.Egg, creatureName);
        Debug.Log($"Game Start {month}/{day}");
        feedManager.AssignPreference();
        Invoke("OhHatched", 3f);
    }
    public void OhHatched()
    {
        currentState = GameState.Hatching;
        uiManager.UpdateCreatueState(GameState.Hatching, creatureName);
        Invoke("ToBaby", 3f);
        Debug.Log("Hatching stage");
    }

    public void ToBaby()
    {
        currentState = GameState.Baby;
        uiManager.UpdateCreatueState(GameState.Baby, creatureName);
        timeManager.SetPaused(false);
        timeManager.ResetGameDay();
        uiManager.ShowGrowthMSG(GameState.Baby, creatureName);
        Debug.Log("Baby stage");
    }

    public void OnGrowToAdult()
    {
        currentState=GameState.Adult;
        uiManager.UpdateCreatueState(GameState.Adult, creatureName);
        uiManager.ShowGrowthMSG(GameState.Adult, creatureName);
        Invoke("ShowPreferenceMessage", 3f);
        uiManager.UpdatePreference();
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
        StartGame(name, month, day);

    }
    public void Feed(string label)
    {
        string formattedLabel = char.ToUpper(label[0]) + label.Substring(1);

        if (Enum.TryParse(formattedLabel, out FoodType food))
        {
            uiManager.UpdateDrawingResult($"{creatureName} ate {label}");
            feedManager.Feed(food);
        }
        else
        {
            Debug.Log($"Failed: {formattedLabel}");
        }
    }
    public void Sleep(String label)
    {
        string formattedLabel = char.ToUpper(label[0]) + label.Substring(1);
        if (formattedLabel == "Bed")
        {
            uiManager.UpdateDrawingResult($"{creatureName} is Sleeping!");
            sleepManager.StartSleep();

        }
        else
        {
            Debug.Log($"Failed: {formattedLabel}");
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

    public void QuiteGame()
    {
        Application.Quit();
    }

    //Test
    [ContextMenu("Test Feed Vegetable")] void TestVegetable() => Feed("Vegetable");
    [ContextMenu("Test Feed Meat")] void TestMeat() => Feed("Meat");
    [ContextMenu("Test Feed Noodle")] void TestNoodle() => Feed("Noodle");
    [ContextMenu("Test Feed Bread")] void TestBread() => Feed("Bread");
    [ContextMenu("Test Feed Drink")] void TestDrink() => Feed("Drink");
    [ContextMenu("Test Sleep")] void TestSleep () => Sleep("Bed");

}
