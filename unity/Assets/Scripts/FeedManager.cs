using System;
using System.Collections.Generic;
using UnityEngine;

public enum FoodType
{
    Vegetable,
    Meat,
    Noodle,
    Bread,
    Drink
}
public enum FoodPreference
{
    Like,
    Dislike,
    Neutral
}

public class FeedManager : MonoBehaviour
{
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SleepManager sleepManager;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private CreatureManager creatureManager;
    private Dictionary<FoodType, FoodPreference> preferences = new();
    List<FoodType> foods = new List<FoodType>();
    private bool isInitialized = false;

    //Getter
    public List<FoodType> Foods => foods;

    public void Initialize()
    {
        if(isInitialized) return;
        isInitialized = true;
        AssignPreference();
    }

    public void AssignPreference()
    {
        preferences.Clear();

        foods = new List<FoodType>((FoodType[])Enum.GetValues(typeof(FoodType)));
        for(int i = foods.Count-1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); ;
            (foods[i], foods[j]) = (foods[j], foods[i]);
        }

        foreach(FoodType food in foods)
        {
            preferences[food] = FoodPreference.Neutral;
        }

        preferences[foods[0]] = FoodPreference.Like;
        preferences[foods[1]] = FoodPreference.Like;
        preferences[foods[2]] = FoodPreference.Dislike;
        preferences[foods[3]] = FoodPreference.Dislike;
        preferences[foods[4]] = FoodPreference.Neutral;

        Debug.Log($"Like: {foods[0]}, {foods[1]}");
        Debug.Log($"Dislike: {foods[2]}, {foods[3]}");
        Debug.Log($"Neutral: {foods[4]}");
    }

    private float GetHungerAmount(FoodType food)
    {
        return food switch
        {
            FoodType.Vegetable => UnityEngine.Random.Range(5f, 25f),
            FoodType.Meat => UnityEngine.Random.Range(20f, 50f),
            FoodType.Noodle => UnityEngine.Random.Range(15f, 40f),
            FoodType.Bread => UnityEngine.Random.Range(10f, 35f),
            FoodType.Drink => UnityEngine.Random.Range(5f, 20f),
            _ => 0f
        };
    }

    private float GetHappinessAmount(FoodType food)
    {
        if(gameManager.State == GameState.Baby)
        {
            return UnityEngine.Random.Range(0f, 15f);
        }

        FoodPreference pref;
        if (preferences.ContainsKey(food))
        {
            pref = preferences[food];
        }
        else
        {
            pref = FoodPreference.Neutral;
        }

        if (pref == FoodPreference.Like) {
            return UnityEngine.Random.Range(25f, 40f);
        }
        if (pref == FoodPreference.Dislike)
        {
            return UnityEngine.Random.Range(-30f, -15f);
        }
        if (pref == FoodPreference.Neutral)
        {
            return UnityEngine.Random.Range(0f, 15f);
        }

        return 0f;

    }

    public void Feed(FoodType food)
    {
        if (sleepManager.IsSleeping) return;
        if(statsManager.Energy <= 0.5f)
        {
            uIManager.UpdateDrawingResult("Too tied to eat.....");
            Debug.Log("Too tried to eat");
            creatureManager.showDislike();
            return;
        }
        if(statsManager.Hunger <= 0.5f)
        {
            statsManager.ApplyOverFeed();
            uIManager.UpdateOverFeed();
            creatureManager.showDislike();
            Debug.Log("Over Feed");
            return;
        }
        float hungerAmount = GetHungerAmount(food);
        float happinessAmount = GetHappinessAmount(food);

        statsManager.ApplyFeed(hungerAmount, happinessAmount);

        if(happinessAmount < 0f)
        {
            creatureManager.showDislike();
        }

        if (happinessAmount >= 25f)
        {
            creatureManager.showlike();
        }
        Debug.Log($"Fedd {food}, Hunger -{hungerAmount:F1}, Happiness {happinessAmount:F1}");
    }


}
