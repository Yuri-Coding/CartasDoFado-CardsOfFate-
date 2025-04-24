using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{

    public GameObject pauseMenu;

    void Update()
    {
        
    }

    public void quitFunction()
    {
        Time.timeScale = 1;
        GameManager.Instance.SwapScene(0);
    }

    public void resumeFunction()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
