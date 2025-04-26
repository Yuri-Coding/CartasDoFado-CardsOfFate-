using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using JetBrains.Annotations;
using NUnit.Framework;

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
	public int currentRound;


	// Texto de Notificação que aparece em toda ShowResults
	string notificationText;

	// Animação de Quadros
	public Animation paintingAnimation;
	public List<TMP_Text> playerNameText;


	// Booleanas de Verificação
	public bool canDraw;
	public bool inPlay;
	public bool alreadyVoted;

	// Lista de jogadores
	private List<Player> playerList;

	// Relacionado a Votação
	private int mostVotedIndex;

	private int votingGap = 3;     // Frequência de Votação (votação / 3 rodadas)
	private int votingModular = 2; // mod3(currentRound)
								   // 0 - Primeira votação acontece no round 3
								   // 1 - Primeira votação acontece no round 1
								   // 2 - Primeira votação acontece no round 2

	private int shopGap = 3;      // Frequência de Votação (votação / 3 rodadas)
	private int shopModular = 2;  // mod3(currentRound)


	// Condição de Vitória / Perda para Player.
	public EndCondition endCondition;

	//tabela de localização
	public LocalizedStringTable NotificationTable;
	public LocalizedStringTable OtherTable;


	// Relacionado a Áudio
	public int tensionIndicator = 0;

	//Lista com os quadros para trocar para os quadros mortos
	public List<Button> Portrait;
	public List<Image> PortraitDead;

	//Vars para controlar a pausa do jogo
	public GameObject pauseMenu;
	public bool isPaused = false;

	//Vars para o puzzle
	bool puzzleControl = false;
	public List<List<string>> currentSeqs = new List<List<string>> {};
	public List <string> inputSeq;
	int NPCIndex = 0;

	//timer do puzzle
	[SerializeField] TextMeshProUGUI timerText;
    public float remainingTime = 7.50F;

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
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        /*if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true)
        {
			isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }*/
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
			case GameState.PuzzlePhase:
				HandleTimer();
				if (puzzleControl)
				{
                    popup.PuzzlePopout();
                    inputSeq = new List<string>();
                    puzzleControl = false;
					remainingTime = 7.50F;
                    OnPuzzleEnd();
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

			case GameState.ShopPhase:
				HandleShopPhase();
				break;

			case GameState.PuzzlePhase:
				HandlePuzzlePhase();
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
		currentRound = 1;

        StringTable table = LocalizationSettings.StringDatabase.GetTable("IntroTable");
        if (table != null)
        {
            // Pegar todas as chaves disponíveis
            List<string> keys = new List<string>(table.SharedData.Entries.Select(entry => entry.Key));

            if (keys.Count > 0)
            {
                int indexString = UnityEngine.Random.Range(0, keys.Count);
                string randomKey = keys[indexString];

                // Buscar e exibir o texto localizado
			popup.SetStateAfterPopup(LocalizationSettings.StringDatabase.GetLocalizedString("IntroTable", randomKey), 20f, GameState.AwaitAction);

            }
        }

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

		// popup.SetStateAfterPopup(introductionTexts[index], 20f, GameState.AwaitAction);
        popup.PopupClosed += SetState;

		AudioManager.Instance.SetMusic(Musics.CantoDaVila);

        List<Roles> rawRoles = new List<Roles>() { Roles.Corrupt, Roles.Medic, Roles.Honest, Roles.Honest, Roles.Honest };
        List<Roles> shuffledRoles = rawRoles.OrderBy(x => Guid.NewGuid()).ToList();

		List<string> rawBotFemaleNames = new List<string>
		{
            "Charlotte",
			"Victoria",
			"Eleanor",
			"Margaret",
			"Florence",
        };

        List<string> rawBotMaleNames = new List<string>
        {
			"Edward",
			"Arthur",
			"Henry",
            "Alfred",
            "Charles",
        };
       

        List<string> shuffledMaleName   = rawBotMaleNames.OrderBy(x => Guid.NewGuid()).ToList();
        List<string> shuffledFemaleName = rawBotFemaleNames.OrderBy(x => Guid.NewGuid()).ToList();

		string playerName = LocalizationSettings.StringDatabase.GetLocalizedString("OthersTable", "player_name");


        Player p1 = new Player(0, shuffledFemaleName[0], shuffledRoles[0], false, true, true);
        Player p2 = new Player(1, playerName,            shuffledRoles[1], true, false, true);
        Player p3 = new Player(2, shuffledMaleName[0],   shuffledRoles[2], false, true, true);
        Player p4 = new Player(3, shuffledMaleName[1],   shuffledRoles[3], false, true, true);
        Player p5 = new Player(4, shuffledFemaleName[1], shuffledRoles[4], false, true, true);

		playerNameText[0].text = shuffledFemaleName[0];
		playerNameText[1].text = shuffledMaleName[0];
		playerNameText[2].text = shuffledMaleName[1];
        playerNameText[3].text = shuffledFemaleName[1];

        playerManager.AddPlayer(p1);
        playerManager.AddPlayer(p2);
        playerManager.AddPlayer(p3);
        playerManager.AddPlayer(p4);
        playerManager.AddPlayer(p5);

        mainPlayer = p2;
        mainRole = mainPlayer.PlayerRole;

        playerManager.InitializePlayers();
		playerManager.InitializeGlobalParameters();

        for (int i = 0; i < 2; i++)
        {
			canDraw = true;
            deckManager.DrawCard(handManager);
        }
		canDraw = true;

    }

    void StartPhase()
	{
		popup.BigTextPopup(currentRound);
        popup.UpdatePoisonIndicator();

        SetState(GameState.AwaitAction);
        UpdateTensionIndicator();
        AudioManager.Instance.ChangeMusicByTensionIndicator(tensionIndicator);
	}

	void AwaitAction()
	{
		// Inscreve-se no evento de ação do jogador
		int index = 0;
		playerList = playerManager.GetAllPlayers();
		foreach (Player jugador in  playerList)
		{
            if (jugador.IsAlive)
				{
					//Quadro activation
					if(index == 0)
					{
						Portrait[index].gameObject.SetActive(true);
						PortraitDead[index].gameObject.SetActive(false);
					}else if (index > 1 && index < 5)
					{
						Portrait[index - 1].gameObject.SetActive(true);
	                    PortraitDead[index - 1].gameObject.SetActive(false);
                }
            }
				else
				{
					//Quadro deactivation
					if(index == 0)
					{
						PortraitDead[index].gameObject.SetActive(true);
						Portrait[index].gameObject.SetActive(false);
					}else if(index >1 && index < 5)
					{
						PortraitDead[index - 1].gameObject.SetActive(true);
						Portrait[index - 1].gameObject.SetActive(false);
					}
			}
			index++;
		}
		paintingAnimation.Play("painting_fadein");
		mainPlayer.OnPlayerAction += OnPlayerActionCompleted;
		Debug.LogWarning($"[FASE{currentRound}] Await Player: Modo espera ativado.");

	}

	private void OnPlayerActionCompleted()
	{
        mainPlayer.OnPlayerAction -= OnPlayerActionCompleted;
		//Debug.Log("GameManager detectou ação");

		SetState(GameState.HandleActions);
	}

	void HandleActions()
	{
        paintingAnimation.Play("painting_fadeout");
        playerManager.HandleBotAction();
		SetState(GameState.ShowResults);
	}

	void HandleShowResults()
	{
        playerManager.VerifyPoisonForAllPlayers();
		VerifyEndGameCondition();
        UpdateUI();

        //Debug.Log("Fase de Mostrar Resultados (Jornal)");
        mainPlayer.CheckoutAllNotifications();

		foreach(Notification notification in mainPlayer.notifications)
		{
			notificationText += (notification.FinalText);
			notificationText += "\n";
		}

		if (!string.IsNullOrWhiteSpace(notificationText))
		{
            popup.SetStateAfterPopup(notificationText, 7f, GameState.ShopPhase);
            popup.PopupClosed += SetState;

            playerManager.ResetNotification();
            notificationText = null;

        } else {
            playerManager.ResetNotification();
            notificationText = null;

            SetState(GameState.ShopPhase);
        }
    }
	void HandleShopPhase()
	{
		if (currentRound%shopGap == shopModular && mainPlayer.PlayerRole != Roles.Corrupt)
		{
			ShopManager.Instance.OpenShop();
            ShopManager.Instance.closeShopAction += OnShopClosed;
        } else {
			SetState(GameState.VotingPhase);
		}
	}
	void OnShopClosed()
	{
		SetState(GameState.PuzzlePhase);
	}

	void HandlePuzzlePhase()
	{
        if (currentRound % shopGap == shopModular && mainPlayer.PlayerRole != Roles.Corrupt)
		{
            currentSeqs = new List<List<string>> { };
            popup.UpdatePuzzleSidePanel();
            popup.PuzzlePopup();
			if(mainPlayer.Corruption <= 0){
				remainingTime += 3.00F;
			}
			Debug.Log("Passei aqui");
			//Funções de como funfa o puzzle
			//Resultado
		}
	}

	void OnPuzzleEnd()
	{
		SetState(GameState.VotingPhase);
	}

	public void AppendSeq( string seq)
	{
		//Se mudar o player de lugar refatora isso por completo
		//Lógica: Começa contando a partir de um, na versão de dev o jogador ocupa a posição 1, portanto torna ela 0 para pegar o primeiro NPC, o resto já vem certo pelo valor do index de npc automaticamente, ou seja o 2 pega o NPC no lugar 2, o 3 no lugar e assim por diante
		NPCIndex = 0;
		inputSeq.Add(seq);
		if(inputSeq.Count >= 6)
		{
			puzzleControl = true;
		}
		foreach(List<string> NPCSeq in currentSeqs)
		{
            NPCIndex++;
            if (NPCSeq.SequenceEqual(inputSeq))
			{
                //Debug.Log(playerList[NPCIndex].PlayerName + " é " + playerList[NPCIndex].PlayerRole + " e seu índice é: " + NPCIndex);
                if (NPCIndex == 1)
				{
					Debug.Log(playerList[0].PlayerName + " é " + playerList[0].PlayerRole + " e seu índice é: 0");
				}
				else
				{
					//Debug.Log(NPCIndex + "dentro do if");
                    Debug.Log(playerList[NPCIndex].PlayerName + " é " + playerList[NPCIndex].PlayerRole + " e seu índice é: " + (NPCIndex));
                }
                puzzleControl = true;
            }
            Debug.Log(NPCIndex);
        }
    }

	void HandleVotingPhase()
	{
		//Debug.Log("Fase de Votação");
		if (currentRound%votingGap == votingModular)
		{
			//Mostra e alimenta o popup de votação
			playerManager.HandleBotVote();
            popup.UpdateVotePanel();
            popup.VotePanelPopup();

		} else {
			// Skipar fase de votação
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
			playerManager.KillPlayerByIndex(mostVotedIndex);
            VerifyEndGameCondition();
			if (mainPlayer.IsAlive) showElimination();
        }
        else if(mostVotedIndex == -1)
		{
			SetState(GameState.EndPhase);		
		}
    }

    void EndPhase() {
		currentRound++;

        //Debug.Log("Fase de Finalização de Turno");
        roundResetVote();

        //Debug.Log(mainPlayer.IsAlive);

        VerifyEndGameCondition();
        UpdateUI();

        SetState(GameState.StartPhase);

		// Calcula o tension indicator

        
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
		if (mainPlayer.IsAlive == false && mainPlayer.PlayerRole != Roles.Corrupt)
        {
            popup.EndGamePopup(EndCondition.SP_PlayerDead);
			SetState(GameState.EndGame);
        }
    }

	void EndGame()
	{
		if (mainPlayer.IsAlive == false && mainPlayer.PlayerRole != Roles.Corrupt) return;
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
		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", "notification_vote_count");
		foreach(Player jugador in playerList)
		{
			voteCount += string.Format(localizedText, jugador.PlayerName, jugador.votesReceived);
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
		// List<string> eliminationMessage;
		int index = 0;

		// eliminationMessage = new List<string> {
        //     $"{playerList[mostVotedIndex].PlayerName} foi eliminado da mesa de negociações.",
		// 	$"{playerList[mostVotedIndex].PlayerName} foi enviado ao oblívio, deixando para trás apenas arrependimentos.",
		// 	$"{playerList[mostVotedIndex].PlayerName} alcançou um destino infeliz.",
		// 	$"{playerList[mostVotedIndex].PlayerName} teve seus gritos de desespero abafados na prisão, em meio ao fim doloroso e sofrido que encontrou, como muitos outros antes e depois dele.",
		// 	$"{playerList[mostVotedIndex].PlayerName} teve um fim prematuro dado a seus sonhos e esperanças.",
		// 	$"Neste teatro cruel, {playerList[mostVotedIndex].PlayerName} assumiu o papel de vítima em uma conspiração fatal.",
		// 	$"{playerList[mostVotedIndex].PlayerName} foi enviado para o além, restando apenas as memórias deixadas para trás."
		// };
		List<string> possibleKeys = new List<string>
		{
			"notification_elimination_1",
			"notification_elimination_2",
			"notification_elimination_3",
			"notification_elimination_4",
			"notification_elimination_5",
			"notification_elimination_6",
			"notification_elimination_7",
		};

		// index = UnityEngine.Random.Range(0, eliminationMessage.Count);
		index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];
		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		popup.SetStateAfterPopup(string.Format(localizedText, playerList[mostVotedIndex].PlayerName), 120f, GameState.EndPhase);
		// popup.SetStateAfterPopup(eliminationMessage[index], 120f, GameState.EndPhase);
        popup.PopupClosed += SetState;
    }

	public void UpdateUI()
	{
		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", "round_counter");
        roundText.text = $"{localizedText} {currentRound.ToString()}";
    }

	public void UpdateTensionIndicator()
	{
		// Variáveis de Balanceamento
		int averageMaxRound = 8;

		int RoundIndicatorWeight  = 10;
		int AliveIndicatorWeight  = 0;
		int PoisonIndicatorWeight = 0;

		// Variáveis Gerais
		int aliveNumber  = playerManager.NumberOfAlive();
		int playerNumber = playerManager.NumberOfPlayers();
		int minPlayer    = 3;


		int mainPlayerPoison = mainPlayer.Poison;
		int maxPoisonLimit   = playerManager.poisonLimit;

		// Formula
		float RoundIndicator  = (float)currentRound / (float)averageMaxRound;
		float AliveIndicator  = 1 - ( ((float)aliveNumber - (float)minPlayer) / ((float)playerNumber - (float)minPlayer) );
		float PoisonIndicator = mainPlayerPoison / maxPoisonLimit;

		float tensionIndicatorFloat =	(RoundIndicator  * (float)RoundIndicatorWeight ) +
										(AliveIndicator  * (float)AliveIndicatorWeight ) +
										(PoisonIndicator * (float)PoisonIndicatorWeight) ;

		tensionIndicator = (int)Math.Floor(tensionIndicatorFloat);
		
		Debug.Log($"currentRound: {currentRound}");
		Debug.Log($"RoundIndicator: {RoundIndicator}, AliveIndicator: {AliveIndicator}, PoisonIndicator = {PoisonIndicator}");
	}

    public void SwapScene(int nextScene)
    {
        AudioManager.Instance.SetMusic(Musics.TerraDoAmanha);
        SceneManager.LoadScene(nextScene);
    }

	public void HandleTimer(){
		if(remainingTime < 0){
			puzzleControl = true;
		}else{
			remainingTime -= Time.deltaTime;
			int seconds = Mathf.FloorToInt(remainingTime);
			int milliseconds = Mathf.FloorToInt((remainingTime - seconds) * 100);
			timerText.text = string.Format("{0}.{1:00}", seconds, milliseconds);
		}

	}
}