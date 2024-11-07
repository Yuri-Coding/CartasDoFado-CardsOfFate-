using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

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

	//Indice do jogador mais votado na rodada
	private int mostVotedIndex;

	// Condição de Vitória / Perda para Player.
	public EndCondition endCondition;

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
		popup.InitPopupMessage();
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
			case GameState.EndGame:
				EndGame();
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
		//Mostra os resultados dos votos
		showVoteResults();
		//Eliminar jogador com mais votos
		mostVoted();
		if (mostVotedIndex >=0)
		{
			playerList[mostVotedIndex].IsAlive = false;
			showElimination();
		}
		SetState(GameState.EndPhase);
	}

    void EndPhase() {
		round++;

        // Condições de Vitória e Derrota
        if (playerManager.NoCorruptAlive())
        {
			endCondition = EndCondition.HonestWin;
			SetState(GameState.EndGame);
			return;
        }

        if (playerManager.IsMostHonestEliminated())
        {
			endCondition = EndCondition.CorruptWin;
			SetState(GameState.EndGame);
			return;
        }

		

        //Debug.Log("Fase de Finalização de Turno");
        roundResetVote();
        SetState(GameState.StartPhase);


        
	}

	void EndGame()
	{
		switch(mainPlayer.PlayerRole)
		{
			case Roles.Honest:
			case Roles.Medic:
				if (endCondition == EndCondition.HonestWin) SetState(GameState.Win);
                if (endCondition == EndCondition.CorruptWin) SetState(GameState.Lose);
                break;


			case Roles.Corrupt:
                if (endCondition == EndCondition.HonestWin) SetState(GameState.Lose);
                if (endCondition == EndCondition.CorruptWin) SetState(GameState.Win);
                break;
        }
	}

	void Win()
	{
		popup.EndGamePopup(endCondition);
	}

	void Lose()
	{
        popup.EndGamePopup(endCondition);
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

	//Função para verificar qual jogador teve mais votos
	private void mostVoted()
	{
		mostVotedIndex=-1;
		//Armazena o número de votos da pessoa que mais recebeu votos na rodada
		int voteCount = 0;

		for(int i=0; i < playerList.Count; i++)
		{
			if (playerList[i].votesReceived > voteCount)
			{
				mostVotedIndex = i;
				voteCount = playerList[i].votesReceived;
			}
		}
	}

    //Popup mostrando quem foi eliminado
    private void showElimination()
    {
		string eliminationMessage;
		eliminationMessage = $"{playerList[mostVotedIndex].PlayerName} foi eliminado da mesa de negociações.";
		popup.PopupMessage(eliminationMessage);
		eliminationMessage = "";
    }

    public void OnInitPopdown()
	{
		List<Roles> rawRoles = new List<Roles>() { Roles.Corrupt, Roles.Medic, Roles.Honest, Roles.Honest, Roles.Honest };
		List<Roles> shuffledRoles = rawRoles.OrderBy(x => Guid.NewGuid()).ToList();
        Player p1 = new Player(0, "Matias", shuffledRoles[0], false, true , true);
		Player p2 = new Player(1, "Cassis", shuffledRoles[1], true , false, true);
		Player p3 = new Player(2, "Yuras" , shuffledRoles[2], false, true , true);
		Player p4 = new Player(3, "Sales" , shuffledRoles[3], false, true , true);
		Player p5 = new Player(4, "Robson", shuffledRoles[4], false, true , true);

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