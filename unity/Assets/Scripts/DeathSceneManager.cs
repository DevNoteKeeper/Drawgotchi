using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSceneManager : MonoBehaviour
{
    public void OnResetButton()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Start");
    }
    public void QuiteGame()
    {
        Application.Quit();
    }
}
