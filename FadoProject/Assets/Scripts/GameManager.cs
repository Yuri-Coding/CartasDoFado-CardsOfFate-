using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Popup popup;
    public enum GameState
    {
        InitGame,
        AwaitPlayer,
        HandlePlayerCard,
        AwaitOther,
        HandleOtherCard,
        EndPhase,
        Win,
        Lose
    }

    public GameState currentState;

    void Start()
    {
        SetState(GameState.InitGame);
        popup.PopupMessage("Bem vindo ao cartas do fado");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Se deseja que o GameManager persista entre cenas
        }
        else
        {
            Destroy(gameObject); // Destruir duplicatas, se houver
        }
    }


    void Update()
    {
        HandleState();
    }

    void SetState(GameState newState)
    {
        currentState = newState;
    }

    void HandleState()
    {
        switch (currentState)
        {
            case GameState.InitGame:
                InitGame();
                break;

            case GameState.AwaitPlayer:
                AwaitPlayer();
                break;

            case GameState.HandlePlayerCard:
                HandlePlayerCard();
                break;

            case GameState.AwaitOther:
                AwaitOther();
                break;

            case GameState.HandleOtherCard:
                HandleOtherCard();
                break;

            case GameState.EndPhase:
                EndPhase();
                break;

            case GameState.Win:
                Win();
                break;

            case GameState.Lose:
                Lose();
                break;

        }
    }

    void InitGame() {
        
        //popup.PopupMessage("Teste!");
    }

    void AwaitPlayer()
    {

    }
    void HandlePlayerCard()
    {

    }
    void AwaitOther()
    {

    }

    void HandleOtherCard()
    {

    }
   
    void EndPhase() { /* Handle end phase logic */ }

    void Win()
    {

    }

    void Lose()
    {

    }

    public void OnInitPopdown()
    {
        SetState(GameState.AwaitPlayer);
        Debug.Log("Mudou!");
    }

}
