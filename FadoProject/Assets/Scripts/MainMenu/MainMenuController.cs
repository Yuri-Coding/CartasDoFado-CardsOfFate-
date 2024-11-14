using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    public Animation anim;
    private AnimationState state;

    private bool isPlaying;
    private int skipPhase = 0;
    void Start()
    {
        isPlaying = true;
        anim.Play("Intro");
        state = anim["Intro"];
    }

    void Update()
    {
        if (!isPlaying) return;
        if (!anim.IsPlaying("Intro"))
        {
            anim.gameObject.SetActive(false);
            isPlaying = false;
        }
        if (Input.anyKeyDown)
        {
            skipPhase++;
            switch(skipPhase)
            {
                case 1:
                    state.time = 4.5f;
                    break;
                case > 1:
                    anim.gameObject.SetActive(false);
                    isPlaying = false;
                    break;
            }
        }
    }
    public void SwapScene(int nextScene)
    {
        SceneManager.LoadScene(nextScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
