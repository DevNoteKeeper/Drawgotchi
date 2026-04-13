using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private float realMinutesPerDay = 15f;
    private float RealSecondsPerGameMinutes => (realMinutesPerDay * 60f) / 1440f;

    private int currentMonth;
    private int currentDay;
    private int currentHour;
    private int currentMinute;
    private int gameDay;

    private float timer;
    private bool isRunning;

    private bool isNight = false;

    private float mealTimer = 0f;
    private bool isMealTime;
    [SerializeField]
    private float mealDurationRealSecond = 60f;


    // Getter
    public int CurrentMonth => currentMonth;
    public int CurrentDay => currentDay;
    public int CurrentHour => currentHour;
    public int CurrentMinute => currentMinute;
    public bool IsNight => isNight;
    public int GameDay => gameDay;

    private void Start()
    {
        StartTime();
    }
    private void Update()
    {
        if (!isRunning) return;
        timer += Time.deltaTime;

        if (timer >= RealSecondsPerGameMinutes)
        {
            timer -= RealSecondsPerGameMinutes;
            AdvanceOneMinute();
         
        }
        MealTimer();
    }
    public void StartTime(int month = 7, int day = 1)
    {
        currentMonth = month;
        currentDay = day;
        currentHour = 7;
        currentMinute = 0;
        isRunning = true;
    }

    private void AdvanceOneMinute()
    {
        currentMinute++;

        if(currentMinute >= 60)
        {
            currentMinute = 0;
            currentHour++;
            CheckNightTime();
        }
        if (currentHour  >= 24)
        {
            currentHour = 0;
            gameDay++;
            DateTime current = new DateTime(2026, currentMonth, currentDay);
            DateTime next = current.AddDays(1);

            currentMonth = next.Month;
            currentDay = next.Day;
        }
    }

    public bool MealTimer()
    {
        TimeSpan breakfast = new TimeSpan(7, 0, 0);
        TimeSpan lunch = new TimeSpan(12, 0, 0);
        TimeSpan dinner = new TimeSpan(18, 0, 0);
        TimeSpan currentTime = new TimeSpan(currentHour, currentMinute, 0);

        // start meal time
        if(!isMealTime && (currentTime == breakfast || currentTime == lunch || currentTime == dinner))
        {
            isMealTime = true;
            mealTimer = 0f;
        }

        // operate mealTimer
        if (isMealTime)
        {
            mealTimer += Time.deltaTime;
            if(mealTimer >= mealDurationRealSecond)
            {
                isMealTime= false;
            }
        }
        
        return isMealTime;
    }

    private void CheckNightTime()
    {
        isNight = (currentHour >= 21 || currentHour < 7);
    }
    public void SetPaused(bool paused)
    {
        isRunning = !paused;
    }
    public void ResetGameDay()
    {
        gameDay = 0;
    }



}
