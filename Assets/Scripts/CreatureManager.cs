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
    //[SerializeField] private Animator egg;
    //[SerializeField] private Animator hatching;

    [SerializeField] private Sprite[] eggs;
    [SerializeField] private Sprite[] hatchings;
    private int frame = 0;
    private int index = 0;
    private int eggSpritePerFrame = 10;
    private int hatchingSpritePerFrame = 20;


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
                EggAnimation();
                return;
            case GameState.Hatching:
                HatchingAnimation();
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

    private void EggAnimation() {
        if (index == eggs.Length) return;
        frame++;
        if (frame < eggSpritePerFrame) return;
        creatureImage.sprite = eggs[index];
        frame = 0;
        index++;
        if(index>= eggs.Length)
        {
            index = 0;
        }
    }

    private void HatchingAnimation()
    {
        if (index == hatchings.Length) return;
        frame++;
        if (frame < hatchingSpritePerFrame) return;
        creatureImage.sprite = hatchings[index];
        frame = 0;
        index++;
        if (index >= hatchings.Length)
        {
            index = 0;
        }
    }
}
