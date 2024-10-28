using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using FadoProject;


public class Popup : MonoBehaviour
{
	private Animation anim;

	public TMP_Text contentObject;
    private bool isWaitingForInput = true;

    public TMP_Text choiceContext;
    public TMP_Text choiceText1;
    public TMP_Text choiceText2;

	public Button choice1Button;
	public Button choice2Button;

    public Animation choiceAnim;

	public Card currentCard;
	public EffectHandler effectHandler;


    void Start()
	{
		anim = gameObject.GetComponent<Animation>();


		// Buscar objetos Choice Screen na cena
        choiceContext = GameObject.Find("ChoiceContext").GetComponent<TMP_Text>();
        choiceText1 = GameObject.Find("Choice1Text").GetComponent<TMP_Text>();
        choiceText2 = GameObject.Find("Choice2Text").GetComponent<TMP_Text>();
		choiceAnim = GameObject.Find("ChoicePopup").GetComponent<Animation>();
    }

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.D))
		{
			Popdown();
		}
	}

	public void PopupMessage(string content)
	{
		anim.Play("fadein");
		contentObject.text = content;

        StartCoroutine(AutoHidePopup(7f));
    }

	void Popdown()
	{
		anim.Play("fadeout");
        isWaitingForInput = true;
        GameManager.Instance.OnInitPopdown();
	}

	private IEnumerator AutoHidePopup(float duration)
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
	}

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
                effectHandler.ApplyTask(currentCard.choice1Effects);
				break;
			case 2:
                effectHandler.ApplyTask(currentCard.choice2Effects);
				break;

        }
		
        PopdownChoice();
	}
}