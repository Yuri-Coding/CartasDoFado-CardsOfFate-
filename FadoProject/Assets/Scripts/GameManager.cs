using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public PlayerManager playerManager;
	public DeckManager deckManager;
	public HandManager handManager;
	public Popup popup;

	public Player currentPlayer;
	public int mainPlayerIndex;
	public int currentIndex = 0;

	public static GameManager Instance { get; private set; }
	void Awake()
	{
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject); // Se deseja que o GameManager persista entre cenas
		} else {
			Destroy(gameObject); // Destruir duplicatas, se houver
		}
	}

	
	public enum GameState
	{
		InitGame,
		NextPlayer,
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
		mainPlayerIndex = 1;
		popup.PopupMessage("Bem vindo ao cartas do fado");
		SetState(GameState.InitGame);
	}

	void Update() { /*HandleState();*/ }
	void SetState(GameState newState) {
		currentState = newState;
		Debug.Log($"O estado mudou para {currentState}.");
		HandleState();
	}

	void HandleState()
	{
		switch (currentState)
		{
			case GameState.InitGame:
				InitGame();
				break;

			case GameState.NextPlayer:
				NextPlayer();
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
		

	}

	void NextPlayer()
	{
		currentPlayer = playerManager.GetNextPlayer();

		if (playerManager.IsMainPlayer(currentPlayer)) {
			deckManager.DrawCard(handManager);
			SetState(GameState.AwaitPlayer);
		} else {
			SetState(GameState.AwaitOther);
		}
		
	}

	void AwaitPlayer()
	{
		Debug.Log($"(playerId: {currentPlayer.PlayerId} == mainPlayerIndex{mainPlayerIndex})");
		if (currentPlayer.PlayerId == mainPlayerIndex)
		{
			deckManager.DrawCard(handManager);

			// Inscreve-se no evento de ação do jogador
			currentPlayer.OnPlayerAction += OnPlayerActionCompleted;
			Debug.Log("Await Player: Modo espera ativado.");
		}
		else
		{
			SetState(GameState.AwaitOther);
		}
	}

	private void OnPlayerActionCompleted()
	{
		currentPlayer.OnPlayerAction -= OnPlayerActionCompleted;
		Debug.Log("GameManager detectou ação");

		SetState(GameState.HandlePlayerCard);
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
		Player p1 = new Player(0, "Matias", false);
		Player p2 = new Player(1, "Cassis", true );
		Player p3 = new Player(2, "Yuras" , false);
		Player p4 = new Player(3, "Sales" , false);
		Player p5 = new Player(4, "Robson", false);
		playerManager.AddPlayer(p1);
		playerManager.AddPlayer(p2);
		playerManager.AddPlayer(p3);
		playerManager.AddPlayer(p4);
		playerManager.AddPlayer(p5);

		playerManager.InitializePlayers();
		currentPlayer = playerManager.GetNextPlayer();

		Debug.Log("Init Finalizado.");
		SetState(GameState.AwaitPlayer);

	}

}