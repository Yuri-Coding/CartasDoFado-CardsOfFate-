using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void SwapScene(int nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
