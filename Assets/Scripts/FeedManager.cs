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
    private Dictionary<FoodType, FoodPreference> preferences = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AssignPreference();
    }
    private void AssignPreference()
    {
        List<FoodType> foods = new List<FoodType>((FoodType[])Enum.GetValues(typeof(FoodType)));
        for(int i = foods.Count-1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); ;
            (foods[i], foods[j]) = (foods[j], foods[i]);
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
            FoodType.Vegetable => Random.Range(5f, 25f),
            FoodType.Meat => Random.Range(20f, 50f),
            FoodType.Noodle => Random.Range(15f, 40f),
            FoodType.Bread => Random.Range(10f, 35f),
            FoodType.Drink => Random.Range(5f, 20f),
            _ => 0f
        };
    }

    private float GetHappinessAmount(FoodType food)
    {
        return preferences[food] switch
        {
            FoodPreference.Like => Random.Range(25f, 40f),
            FoodPreference.Dislike => Random.Range(-30f, -15f),
            FoodPreference.Neutral => Random.Range(0f, 15f),
            _ => 0f
        };
    }

    public void Feed(FoodType food)
    {
        if(statsManager.Hunger <= 0f)
        {
            statsManager.ApplyOverFeed();
            Debug.Log("Over Feed");
            return;
        }
        float hungerAmount = GetHungerAmount(food);
        float happinessAmount = GetHappinessAmount(food);

        statsManager.ApplyFeed(hungerAmount, happinessAmount);
        Debug.Lost($"Fedd {food}, Hunger -{hungerAmount:F1}, Happiness {happinessAmount:F1}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
