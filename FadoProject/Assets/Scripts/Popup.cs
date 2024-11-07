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


    // Texto Debug de Parâmetro
    public TMP_Text corruptionText;
    public TMP_Text moraleText;
    public TMP_Text influenceText;

    // Texto Grande que Aparece no Início dos Rounds
    public TMP_Text bigRoundText;
    public Animation bigTextPanelAnimation;

    private Card currentCard;
	public EffectHandler effectHandler;

    //Popup de votação
    public Animation voteAnim;
    //Texto para colocar o nome dos player no botão
    public List<TMP_Text> PText;
    //Pegando os botões
    public List<Button> PButton;

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

    public void PopupMessage(string content)
    {
        singleAnim.Play("fadein");
        contentObject.text = content;

        StartCoroutine(AutoHidePopup(7f, "normal"));
    }

    void Popdown()
    {
        singleAnim.Play("fadeout");
        isWaitingForInput = true;
    }

    // ===============================================================
    //                         FIRST POPUP
    // ===============================================================
    public void InitPopupMessage(string content)
    {
        singleAnim.Play("fadein");
        contentObject.text = content;

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
        if (option == "init") GameManager.Instance.OnInitPopdown();
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
        moraleText.text		= GameManager.Instance.mainPlayer.Morale.ToString();
        influenceText.text	= GameManager.Instance.mainPlayer.Influence.ToString();
	}

    // ===============================================================
    //                          
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
                PButton[index].gameObject.SetActive(true) ;
                PText[index].text = jugador.PlayerName;
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