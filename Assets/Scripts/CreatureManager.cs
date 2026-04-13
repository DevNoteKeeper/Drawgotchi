using UnityEngine;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour
{
    [SerializeField] private StatsManager statsManager;
    [SerializeField] private SleepManager sleepManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Image creatureImage;

    [Header("Baby")]
    [SerializeField] private Sprite babyNormal;
    [SerializeField] private Sprite babyHungry;
    [SerializeField] private Sprite babySleepy;
    [SerializeField] private Sprite babySleep;
    [SerializeField] private Sprite babyHappy;
    [SerializeField] private Sprite babyDislike;

    [Header("Adult")]
    [SerializeField] private Sprite adultNormal;
    [SerializeField] private Sprite adultHungry;
    [SerializeField] private Sprite adultSleepy;
    [SerializeField] private Sprite adultSleep;
    [SerializeField] private Sprite adultHappy;
    [SerializeField] private Sprite adultDislike;

    [Header("Common")]
    [SerializeField] private Sprite egg;
    [SerializeField] private Sprite hatching;


    // Update is called once per frame
    void Update()
    {
        UpdateCreatureSprite();
    }

    private void UpdateCreatureSprite()
    {
        switch (gameManager.State)
        {
            case GameState.Egg:
                creatureImage.sprite = egg;
                return;
            case GameState.Hatching:
                creatureImage.sprite = hatching;
                return;

        }

        bool isBaby = gameManager.State == GameState.Baby;

        if (sleepManager.IsSleeping)
        {
            creatureImage.sprite = isBaby ? babySleep : adultSleep;
            return;
        }

        if (statsManager.Hunger >= 70)
        {
            creatureImage.sprite = isBaby ? babyHungry : adultHungry;

        }
        else if (statsManager.Energy <= 30)
        {
            creatureImage.sprite = isBaby ? babySleepy : adultSleepy;
        }
        else
        {
            creatureImage.sprite = isBaby ? babyNormal : adultNormal;
        }
    }
}
