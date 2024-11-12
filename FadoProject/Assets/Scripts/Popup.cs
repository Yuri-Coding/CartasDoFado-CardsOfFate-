using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using FadoProject;
using Unity.VisualScripting;


public class Popup : MonoBehaviour
{
    // Componentes de Popup Simples
	public Animation singleAnim;

	public TMP_Text contentObject;
    private bool isWaitingForInput = true;


    // Componentes de Popup de Escolha
    public TMP_Text choiceContext;
    public TMP_Text choiceText1;
    public TMP_Text choiceText2;

	public Button choice1Button;
	public Button choice2Button;

    public Animation choiceAnim;

    //Flag para controlar o PopUp que muda state
    public bool isProcessingPopup;

    // Texto Debug de Parâmetro
    public TMP_Text poisonText;
    public TMP_Text moraleText;
    public TMP_Text influenceText;
    public TMP_Text corruptionText;


    // Texto Grande que Aparece no Início dos Rounds
    public TMP_Text bigRoundText;
    public Animation bigTextPanelAnimation;

    private Card currentCard;
	public EffectHandler effectHandler;

    // Texto Grande de notificação de Kill
    public TMP_Text bigKillNotificationText;


    // Texto de EndGame
    public TMP_Text endGameText;
    public TMP_Text endGameDescriptionText;

    public event Action<GameState> PopupClosed;



    //Popup de votação
    public Animation voteAnim;
    //Texto para colocar o nome dos player no botão
    public List<TMP_Text> PText;
    //Pegando os botões
    public List<Button> PButton;
    //Pegando o texto substituto
    public List<TMP_Text> PSubText;
    //Pegando os objetos em baixo que são os textos com risco
    public List<GameObject> PSub;

    //Importando PlayerManager
    public PlayerManager playerManager;
    //Criando uma lista para armazenar a lista de player
    private List<Player> playerList;


	//Criando um evento para ser ouvido
	public event Action actionRemoveCard;

    void Start()
	{


		// Buscar objetos Choice Screen na cena
        choiceContext = GameObject.Find("ChoiceContext").GetComponent<TMP_Text>();
        choiceText1 = GameObject.Find("Choice1Text").GetComponent<TMP_Text>();
        choiceText2 = GameObject.Find("Choice2Text").GetComponent<TMP_Text>();
		choiceAnim = GameObject.Find("ChoicePopup").GetComponent<Animation>();
        // Desativando os botões inicialmente
        for (int i=0; i<PButton.Count; i++)
        {
            PButton[i].gameObject.SetActive(false);
        }
        isProcessingPopup = false;
    }

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.D))
		{
			Popdown();
		}
	}

    // ===============================================================
    //                        GENERAL POPUP
    // ===============================================================

    public void PopupMessage(string content, float duration)
    {
        singleAnim.Play("fadein");
        contentObject.text = content;

        StartCoroutine(AutoHidePopup(duration, "normal"));
    }

    // Popup and Change State
    public void SetStateAfterPopup(string content, float duration, GameState state)
    {
        singleAnim.Play("fadein");
        contentObject.text = content;

        StartCoroutine(AutoHideAndChangeState(duration, state));
    }

    void Popdown()
    {
        singleAnim.Play("fadeout");
        isWaitingForInput = true;
    }

    private IEnumerator AutoHideAndChangeState(float duration, GameState state)
    {
        float elapsedTime = 0f;

        while (isWaitingForInput && elapsedTime < duration)
        {
            if (Input.anyKeyDown)
            {
                isWaitingForInput = false;
                break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        Popdown();
        while (elapsedTime < 1.5f) { elapsedTime+=Time.deltaTime; yield return null; }
        PopupClosed?.Invoke(state);
    }

    // ===============================================================
    //                         FIRST POPUP
    // ===============================================================
    public void InitPopupMessage()
    {
        List<string> introductionTexts = new List<string>()
        {
            "Bem vindo ao Cartas do Fado. A Mesa está preenchida, os olhares, desconfiantes, observam uns aos outros, em busca de encontrar o nocivo, achar um grão de ouro em auto-mar.",
            "A mesa está completa. Cada olhar carrega uma sombra de dúvida e desconfiança, enquanto os jogadores, ocultos por segredos, preparam suas cartas. A noite promete revelar verdades - ou esconder mentiras.",
            "Bem-vindo ao Cartas do Fado, onde cada movimento pode mudar o eterno destino da cidade. À mesa, risos e suspeitas se entrelaçam, mas apenas um saberá a verdade antes de todos os outros. Quem será o primeiro a cair?",
            "As cartas estão postas e as intenções, veladas. Em um jogo de sorte e manipulação, você está cercado por aliados ou inimigos disfarçados. Restará ao destino revelar quem realmente merece confiança.",
            "Hoje, a mesa é palco de um jogo de segredos e conspirações. As cartas sussurram promessas de poder, mas apenas quem conhece os próprios limites escapará ileso.",
        };

        int index = UnityEngine.Random.Range(0, introductionTexts.Count);

        singleAnim.Play("fadein");
        contentObject.text = introductionTexts[index];

        StartCoroutine(AutoHidePopup(7f, "init"));
    }

    private IEnumerator AutoHidePopup(float duration, string option)
	{
		float elapsedTime = 0f;

		while (isWaitingForInput && elapsedTime < duration) {
			if (Input.anyKeyDown) {
				isWaitingForInput = false;
				break;
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		Popdown();
        //if (option == "init") GameManager.Instance.OnInitPopdown();
	}

    // ===============================================================
    //                        CHOICE POPUP
    // ===============================================================
    public void PopupChoice(Card cardData)
	{
		currentCard = cardData;
        choiceAnim.Play("fadein");
        choiceContext.text = currentCard.cardLore;
        choiceText1.text = currentCard.choice1;
        choiceText2.text = currentCard.choice2;

        choice1Button.onClick.AddListener(() => OnChoiceMade(1));
        choice2Button.onClick.AddListener(() => OnChoiceMade(2));
    }

    public void PopdownChoice()
    {
        choice1Button.onClick.RemoveAllListeners();
		choice2Button.onClick.RemoveAllListeners();
        choiceAnim.Play("fadeout");
    }

	public void OnChoiceMade(int choice)
	{
		Debug.Log($"Escolha {choice} selecionada");
        switch (choice)
		{
			case 1:
                effectHandler.ApplyMain(currentCard.choice1Effects);
				break;
			case 2:
                effectHandler.ApplyMain(currentCard.choice2Effects);                
                break;

        }
        PopdownChoice();
		//Chama todas as funções que ficam registradas na ação(ouvindo)
		actionRemoveCard?.Invoke();
	}

    // ===============================================================
    //                         UPDATE PANEL
    // ===============================================================
    public void UpdatePanel()
	{
        corruptionText.text = GameManager.Instance.mainPlayer.Corruption.ToString();
        poisonText.text     = GameManager.Instance.mainPlayer.Poison.ToString();
        moraleText.text		= GameManager.Instance.mainPlayer.Morale.ToString();
        influenceText.text	= GameManager.Instance.mainPlayer.Influence.ToString();
        
	}

    // ===============================================================
    //                          BIG TEXT
    // ===============================================================
    public void BigTextPopup(int round)
    {
        bigRoundText.text = "RODADA " + round.ToString();
        bigTextPanelAnimation.Play("bigtext_fadein");
        StartCoroutine(AutoHideBigText(3f));
    }
    
    public void BigTextPopdown()
    {
        bigTextPanelAnimation.Play("bigtext_fadeout");
    }

    private IEnumerator AutoHideBigText(float duration)
    {
        float elapsedTime = 0f;

        while (isWaitingForInput && elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        BigTextPopdown();
    }

    // =

    public void BigKillNotificationPopup(int round)
    {
        bigRoundText.text = "RODADA " + round.ToString();
        bigTextPanelAnimation.Play("bigtext_fadein");
        StartCoroutine(AutoHideBigText(3f));
    }

    public void BigKillNotificationPopdown()
    {
        bigTextPanelAnimation.Play("bigtext_fadeout");
    }


    // ===============================================================
    //                            END GAME 
    // ===============================================================

    public void EndGamePopup(EndCondition endCondition)
    {
        int coinToss;
        string endGameString = "";
        string endGameDescriptionString = "";
        if (endCondition == EndCondition.SP_PlayerDead)
        {
            coinToss = UnityEngine.Random.Range(0, 2);
            switch ((coinToss, GameManager.Instance.mainRole))
            {
                case (0, Roles.Honest):
                    endGameString = "VOCÊ VENCEU";
                    endGameDescriptionString = "Apesar de seu destino ter sido cruel, os habitantes da cidade sobreviveram ao grande desafio que encararam.";
                    break;
                case (0, Roles.Medic):
                    endGameString = "VOCÊ VENCEU";
                    endGameDescriptionString = "Seus esforços foram recompensados. A cidade prevaleceu e sobreviveu ao ataque do cruel corrupto.";
                    break;
                case (1, Roles.Honest):
                    endGameString = "VOCÊ FOI DERROTADO";
                    endGameDescriptionString = "Mesmo sendo uma pessoa honesta e esforçada o corrupto ceifou sua vida e de toda a cidade.";
                    break;
                case (1, Roles.Medic):
                    endGameString = "VOCÊ FOI DERROTADO";
                    endGameDescriptionString = "Você foi o último raio de esperança da cidade, no entanto o corrupto teve sucesso em apagar essa luz.";
                    break;
            }
        }
        switch((endCondition, GameManager.Instance.mainRole))
        {
            case (EndCondition.HonestWin, Roles.Honest):
                endGameString = "VOCÊ VENCEU";
                endGameDescriptionString = "A honestidade venceu. Todos tinham a oportunidade de desviar-se do bom caminho, mas ao fim, todos trabalharam em prol da paz.";
                break;
            case (EndCondition.HonestWin, Roles.Medic):
                endGameString = "VOCÊ VENCEU";
                endGameDescriptionString = "Você teve um papel essencial nesta grande vitória. Mesmo o trabalho não recompensando-o por reputação, você seguiu a justiça.";
                break;
            case (EndCondition.HonestWin, Roles.Corrupt):
                endGameString = "VOCÊ FOI DERROTADO";
                endGameDescriptionString = "O poder do veneno não foi suficiente para corromper uma sociedade. Um bom trabalho, mas infelizmente, você falhou.";
                break;

            case (EndCondition.CorruptWin, Roles.Honest):
                endGameString = "VOCÊ FOI DERROTADO";
                endGameDescriptionString = "Os esforços, nem mesmo pelo poder da honestidade, foi capaz de dar frutos para combater a corrupção. A corrupção dominou a cidade, e a destruiu.";
                break;
            case (EndCondition.CorruptWin, Roles.Medic):
                endGameString = "VOCÊ FOI DERROTADO";
                endGameDescriptionString = "Como médico, você buscou curar ao máximo as pessoas. Mas o veneno letal da corrupção espalhou mais rápido, resultando no obscuro futuro sem fim da cidade.";
                break;
            case (EndCondition.CorruptWin, Roles.Corrupt):
                endGameString = "VOCÊ VENCEU";
                endGameDescriptionString = "A estratégia miticulosa de corrupção geraram frutos em forma de destruição. Tudo que restou na cidade foram as memórias de bons tempos, destruição e desesperança.";
                break;
        }
        

        endGameText.text = endGameString;
        endGameDescriptionText.text = endGameDescriptionString;

        bigTextPanelAnimation.Play("endgame_fadein");
    }



    // ===============================================================
    //                          VOTE POPUP
    // ===============================================================
    public void UpdateVotePanel()
    {
        int index=0;
        playerList = playerManager.GetAllPlayers();
        foreach (Player jugador in playerList)
        {
            if (jugador.PlayerName != "")
            {
                if (jugador.IsAlive)
                {
                    PButton[index].gameObject.SetActive(true);
                    PText[index].text = jugador.PlayerName;
                }
                else
                {
                    PButton[index].gameObject.SetActive(false);
                    PSub[index].gameObject.SetActive(true);
                    PSubText[index].text=jugador.PlayerName;
                }
                
            }
            index++;
        }
    }

    public void VotePanelPopup()
    {
        voteAnim.Play("fadeIn");
    }
    public void VotePanelPopout()
    {
        voteAnim.Play("fadeOut");
    }
}