using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
	// Managers
	public PlayerManager playerManager;
	public DeckManager deckManager;
	public HandManager handManager;
	public Popup popup;

	// Identificação do Player principal (o que está jogando na máquina)
    public Player mainPlayer;
	public Roles mainRole;
    public int mainPlayerIndex;

	// Elemento de Round
    public TMP_Text roundText;
    public int round;


    // Texto de Notificação que aparece em toda ShowResults
    string notificationText;

    


	public bool canDraw;

	//Var para verificar se carta está sendo jogada
	public bool inPlay;

    //Var para verificar se o jogador já votou.
    public bool alreadyVoted;

	//Lista de jogadores
	private List<Player> playerList;

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
		popup.InitPopupMessage("Bem vindo ao cartas do fado");
		SetState(GameState.InitGame);
		canDraw = true;
		inPlay = false;
		alreadyVoted = false;
		playerList = playerManager.GetAllPlayers();
	}

	void Update()
	{
		switch (currentState)
		{
			case GameState.VotingPhase:
                if (alreadyVoted)
                {
                    popup.VotePanelPopout();
                    alreadyVoted = false;
                    SetState(GameState.ProcessVoteResults);
                }
				break;
        }
	}

	void SetState(GameState newState) {
		currentState = newState;
		//Debug.Log($"O estado mudou para {currentState}.");
		HandleState();
	}

	void HandleState()
	{
		switch (currentState)
		{
			case GameState.InitGame:
				InitGame();
				break;

			case GameState.StartPhase:
				StartPhase();
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
				canDraw = true;
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
		round = 1;
	}

	void StartPhase()
	{
		popup.BigTextPopup(round);

		SetState(GameState.AwaitAction);
	}

	void AwaitAction()
	{
		// Inscreve-se no evento de ação do jogador
		mainPlayer.OnPlayerAction += OnPlayerActionCompleted;
		Debug.LogWarning($"[FASE{round}] Await Player: Modo espera ativado.");

	}

	private void OnPlayerActionCompleted()
	{
		mainPlayer.OnPlayerAction -= OnPlayerActionCompleted;
		//Debug.Log("GameManager detectou ação");

		SetState(GameState.HandleActions);
	}

	void HandleActions()
	{
		playerManager.HandleBotAction();
		SetState(GameState.ShowResults);
	}

	void HandleShowResults()
	{
		//Debug.Log("Fase de Mostrar Resultados (Jornal)");
		//TOMAS SE PÁ É MELHOR N MOSTRA NADA QUANDO A NOTIFICAÇÃO ESTIVER VAZIA, VULGO SEM NOTIFICAÇÃO, TO CORINGANDO AQ COM ESSE PAPEL VAZIO KKKKKK
		
		mainPlayer.CheckoutAllNotifications();

		foreach(Notification notification in mainPlayer.notifications)
		{
			notificationText += (notification.FinalText);
			notificationText += "\n";
		}

		if (!string.IsNullOrWhiteSpace(notificationText))
		{
			popup.PopupMessage(notificationText);
		}

		playerManager.ResetNotification();

		notificationText = null;

		SetState(GameState.VotingPhase);
	}

	void HandleVotingPhase()
	{
		//Debug.Log("Fase de Votação");
		popup.UpdateVotePanel();
		popup.VotePanelPopup();
	}

	void HandleProcessVoteResults()
	{
		//Debug.Log("Fase de Contagem de Votos");
		showVoteResults();
		//Eliminar jogador com mais votos
		SetState(GameState.EndPhase);
	}

    void EndPhase() {
		round++;
		//Debug.Log("Fase de Finalização de Turno");
		roundResetVote();
        SetState(GameState.StartPhase);

		// Condições de Vitória e Derrota
	}

	void Win()
	{

	}

	void Lose()
	{

	}

	//Função pra resetar os votos que cada player recebeu
    public void roundResetVote()
    {
        foreach (Player jugador in playerList)
        {
            jugador.votesReceived = 0;
        }
    }

	//Função para mostrar os votos que cada player recebeu no popup
    private void showVoteResults()
    {
		string voteCount;
		voteCount = "";
		foreach(Player jugador in playerList)
		{
			voteCount = voteCount + $"{jugador.PlayerName} recebeu {jugador.votesReceived} voto(s)\n";
		}
		popup.PopupMessage(voteCount);

		voteCount = "";
    }

    public void OnInitPopdown()
	{
		Player p1 = new Player(0, "Matias", Roles.Honest,  false, true );
		Player p2 = new Player(1, "Cassis", Roles.Corrupt, true , false);
		Player p3 = new Player(2, "Yuras" , Roles.Medic,   false, true );
		Player p4 = new Player(3, "Sales" , Roles.Honest,  false, true );
		Player p5 = new Player(4, "Robson", Roles.Honest,  false, true );

		playerManager.AddPlayer(p1);
		playerManager.AddPlayer(p2);
		playerManager.AddPlayer(p3);
		playerManager.AddPlayer(p4);
		playerManager.AddPlayer(p5);

        mainPlayer = p2;
        mainRole = mainPlayer.PlayerRole;

        playerManager.InitializePlayers();

		
		

		//Debug.Log("Init Finalizado.");
		SetState(GameState.StartPhase);

	}

	public void UpdateUI()
	{
        roundText.text = round.ToString();
    }
}