using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public PlayerManager playerManager;
	public DeckManager deckManager;
	public HandManager handManager;
	public Popup popup;

    public Player mainPlayer;

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

			case GameState.AwaitAction:
				AwaitAction();
				break;

			case GameState.HandleActions:
				HandleActions();
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

	void AwaitAction()
	{
		deckManager.DrawCard(handManager);

		// Inscreve-se no evento de ação do jogador
		mainPlayer.OnPlayerAction += OnPlayerActionCompleted;
		Debug.Log("Await Player: Modo espera ativado.");

	}

	private void OnPlayerActionCompleted()
	{
		mainPlayer.OnPlayerAction -= OnPlayerActionCompleted;
		Debug.Log("GameManager detectou ação");

		SetState(GameState.HandleActions);
	}

	void HandleActions()
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
		Player p1 = new Player(0, "Matias", false, true );
		Player p2 = new Player(1, "Cassis", true , false);
		Player p3 = new Player(2, "Yuras" , false, true );
		Player p4 = new Player(3, "Sales" , false, true );
		Player p5 = new Player(4, "Robson", false, true );
		playerManager.AddPlayer(p1);
		playerManager.AddPlayer(p2);
		playerManager.AddPlayer(p3);
		playerManager.AddPlayer(p4);
		playerManager.AddPlayer(p5);

		playerManager.InitializePlayers();
		mainPlayer = p2;

		Debug.Log("Init Finalizado.");
		SetState(GameState.AwaitAction);

	}

}