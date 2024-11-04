using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
	public PlayerManager playerManager;
	public DeckManager deckManager;
	public HandManager handManager;
	public Popup popup;

    public Player mainPlayer;
	public Roles mainRole;

    public int mainPlayerIndex;
	public int currentIndex = 0;

	public int round = 1;

    public TMP_Text roundText;

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

            case GameState.ShowResults:
                HandleShowResults();
                break;

			case GameState.VotingPhase:
				HandleVotingPhase();
				break;

			case GameState.ProcessVoteResults:
				HandleProcessVoteResults();
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
		playerManager.HandleBotAction();
		SetState(GameState.ShowResults);
	}

	void HandleShowResults()
	{
		Debug.Log("Fase de Mostrar Resultados (Jornal)");
		SetState(GameState.VotingPhase);
	}

	void HandleVotingPhase()
	{
		Debug.Log("Fase de Votação");
		SetState(GameState.ProcessVoteResults);
	}

	void HandleProcessVoteResults()
	{
		Debug.Log("Fase de Contagem de Votos");
		SetState(GameState.EndPhase);
	}

	void EndPhase() {
		round++;
		Debug.Log("Fase de Finalização de Turno");
		SetState(GameState.AwaitAction);
	}

	void Win()
	{

	}

	void Lose()
	{

	}

	public void OnInitPopdown()
	{
		Player p1 = new Player(0, "Matias", Roles.Corrupt, false, true );
		Player p2 = new Player(1, "Cassis", Roles.Honest,  true , false);
		Player p3 = new Player(2, "Yuras" , Roles.Medic,   false, true );
		Player p4 = new Player(3, "Sales" , Roles.Honest,  false, true );
		Player p5 = new Player(4, "Robson", Roles.Honest,  false, true );

		playerManager.AddPlayer(p1);
		playerManager.AddPlayer(p2);
		playerManager.AddPlayer(p3);
		playerManager.AddPlayer(p4);
		playerManager.AddPlayer(p5);

		playerManager.InitializePlayers();

		mainPlayer = p2;
		mainRole = mainPlayer.PlayerRole;
		

		Debug.Log("Init Finalizado.");
		SetState(GameState.AwaitAction);

	}

	public void UpdateUI()
	{
        roundText.text = round.ToString();
    }
}