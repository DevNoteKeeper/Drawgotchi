using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Dropdown monthInput;
    [SerializeField] private TMP_Dropdown dayInput;


    private void Start()
    {
        monthInput.ClearOptions();
        monthInput.options.Add(new TMP_Dropdown.OptionData("Month"));
        for (int i = 1; i <= 12; i++)
        {
            monthInput.options.Add(new TMP_Dropdown.OptionData($"{i}"));
        }
        monthInput.value = 0;
        monthInput.RefreshShownValue();

        dayInput.ClearOptions();
        dayInput.options.Add(new TMP_Dropdown.OptionData("Day"));
        for (int i = 1; i <= 31; i++)
        {
            dayInput.options.Add(new TMP_Dropdown.OptionData($"{i}"));
        }
        dayInput.value = 0;
        dayInput.RefreshShownValue();
    }
    public void onStartButton()
    {
        if (!IsValidInput()) return;

        PlayerPrefs.SetString("name", nameInput.text);
        PlayerPrefs.SetInt("month", monthInput.value);
        PlayerPrefs.SetInt("day", dayInput.value);

        Debug.Log($"Creature: {nameInput.text}, {monthInput.value}/{dayInput.value}");

        SceneManager.LoadScene("Main");
    }

    private bool IsValidInput()
    {
        if (string.IsNullOrEmpty(nameInput.text))
        {
            Debug.Log("Enter Name");
            return false;
        }
        if(monthInput.value == 0)
        {
            Debug.Log("Select Month");
            return false;
        }
        if (dayInput.value == 0)
        {
            Debug.Log("Select Day");
            return false;
        }
        return true;
    }
}
