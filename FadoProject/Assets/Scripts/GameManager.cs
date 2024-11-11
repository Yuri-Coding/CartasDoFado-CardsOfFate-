﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Reflection;

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
        popup.PopupClosed -= SetState;

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

			case GameState.ShowElimination:
				HandleShowElimination();
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

        // Selecionar um texto Lore de entrada
        List<string> introductionTexts = new List<string>()
        {
            "Bem vindo ao Cartas do Fado. A Mesa está preenchida, os olhares, desconfiantes, observam uns aos outros, em busca de encontrar o nocivo, achar um grão de ouro em auto-mar.",
            "A mesa está completa. Cada olhar carrega uma sombra de dúvida e desconfiança, enquanto os jogadores, ocultos por segredos, preparam suas cartas. A noite promete revelar verdades - ou esconder mentiras.",
            "Bem-vindo ao Cartas do Fado, onde cada movimento pode mudar o eterno destino da cidade. À mesa, risos e suspeitas se entrelaçam, mas apenas um saberá a verdade antes de todos os outros. Quem será o primeiro a cair?",
            "As cartas estão postas e as intenções, veladas. Em um jogo de sorte e manipulação, você está cercado por aliados ou inimigos disfarçados. Restará ao destino revelar quem realmente merece confiança.",
            "Hoje, a mesa é palco de um jogo de segredos e conspirações. As cartas sussurram promessas de poder, mas apenas quem conhece os próprios limites escapará ileso.",
        };

        int index = UnityEngine.Random.Range(0, introductionTexts.Count);

		popup.SetStateAfterPopup(introductionTexts[index], 20f, GameState.AwaitAction);
        popup.PopupClosed += SetState;

        List<Roles> rawRoles = new List<Roles>() { Roles.Corrupt, Roles.Medic, Roles.Honest, Roles.Honest, Roles.Honest };
        List<Roles> shuffledRoles = rawRoles.OrderBy(x => Guid.NewGuid()).ToList();

        Player p1 = new Player(0, "Matias", shuffledRoles[0], false, true, true);
        Player p2 = new Player(1, "Cassis", shuffledRoles[1], true, false, true);
        Player p3 = new Player(2, "Yuras", shuffledRoles[2], false, true, true);
        Player p4 = new Player(3, "Sales", shuffledRoles[3], false, true, true);
        Player p5 = new Player(4, "Robson", shuffledRoles[4], false, true, true);

        playerManager.AddPlayer(p1);
        playerManager.AddPlayer(p2);
        playerManager.AddPlayer(p3);
        playerManager.AddPlayer(p4);
        playerManager.AddPlayer(p5);

        mainPlayer = p2;
        mainRole = mainPlayer.PlayerRole;

        playerManager.InitializePlayers();

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
		VerifyEndGameCondition();

		//Debug.Log("Fase de Mostrar Resultados (Jornal)");
		mainPlayer.CheckoutAllNotifications();

		foreach(Notification notification in mainPlayer.notifications)
		{
			notificationText += (notification.FinalText);
			notificationText += "\n";
		}

		if (!string.IsNullOrWhiteSpace(notificationText))
		{
            popup.SetStateAfterPopup(notificationText, 7f, GameState.VotingPhase);
            popup.PopupClosed += SetState;

            playerManager.ResetNotification();
            notificationText = null;

        } else {
            playerManager.ResetNotification();
            notificationText = null;

            SetState(GameState.VotingPhase);
        }	
	}

	void HandleVotingPhase()
	{
		//Debug.Log("Fase de Votação");
		if (round>1)
		{
            //Mostra e alimenta o popup de votação
            popup.UpdateVotePanel();
            popup.VotePanelPopup();
        }else if (round==1) {
			SetState(GameState.EndPhase);
		}
	}

	void HandleProcessVoteResults()
	{
		//Debug.Log("Fase de Contagem de Votos");
		//Mostra os resultados dos votos
		showVoteResults();
	}

	void HandleShowElimination()
	{
        //Eliminar jogador com mais votos
        mostVoted();
        if (mostVotedIndex >= 0)
        {
            playerList[mostVotedIndex].IsAlive = false;
            showElimination();
        }
    }

    void EndPhase() {
		round++;

        //Debug.Log("Fase de Finalização de Turno");
        roundResetVote();
        VerifyEndGameCondition();
        UpdateUI();
        SetState(GameState.StartPhase);


        
	}

	void VerifyEndGameCondition()
	{
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
        popup.SetStateAfterPopup(voteCount, 120f, GameState.ShowElimination);
        popup.PopupClosed += SetState;

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
		List<string> eliminationMessage;
		int index = 0;

		eliminationMessage = new List<string> {
            $"{playerList[mostVotedIndex].PlayerName} foi eliminado da mesa de negociações.",
			$"{playerList[mostVotedIndex].PlayerName} foi enviado ao oblívio, deixando para trás apenas arrependimentos.",
			$"{playerList[mostVotedIndex].PlayerName} alcançou um destino infeliz.",
			$"{playerList[mostVotedIndex].PlayerName} teve seus gritos de desespero abafados na prisão, em meio ao fim doloroso e sofrido que encontrou, como muitos outros antes e depois dele.",
			$"{playerList[mostVotedIndex].PlayerName} teve um fim prematuro dado a seus sonhos e esperanças.",
			$"Neste teatro cruel, {playerList[mostVotedIndex].PlayerName} assumiu o papel de vítima em uma conspiração fatal.",
			$"{playerList[mostVotedIndex].PlayerName} foi enviado para o além, restando apenas as memórias deixadas para trás."
		};

		index = UnityEngine.Random.Range(0, eliminationMessage.Count);
		popup.SetStateAfterPopup(eliminationMessage[index], 120f, GameState.EndPhase);
        popup.PopupClosed += SetState;
    }

	public void UpdateUI()
	{
        roundText.text = round.ToString();
    }
}