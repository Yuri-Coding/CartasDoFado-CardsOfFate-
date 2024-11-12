using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/*
 * menu principal, contém variaiveis que vão ser enviadas pras outras telas
 */
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField nameInput;


    public static string ipAddress;
    public static bool isHost;
    public static string nameString;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() =>
        {
            hostFunction();
        });

        clientBtn.onClick.AddListener(() =>
        {
            clientFunction();
        });
    }

    private void hostFunction()
    {
        if (!CheckNameField())
        {
            isHost = true;
            swapScene(1);
            Debug.Log("Start Game as Host");

        }
    }

    private void clientFunction()
    {
        ipAddress = ipInput.text;
        if (!CheckNameField() && !string.IsNullOrEmpty(ipAddress))
        {
            isHost = false;
            swapScene(1);
            Debug.Log("Start Game as Client");

        }

    }

    private bool CheckNameField()
    {
        nameString = nameInput.text;
        return string.IsNullOrEmpty(nameString);
    }

    public void swapScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
