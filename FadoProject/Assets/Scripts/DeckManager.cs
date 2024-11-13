using FadoProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
	public List<Card> allCards = new List<Card>();

	public int currentIndex = 0;
    public static DeckManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Se deseja que o GameManager persista entre cenas
        }
        else
        {
            Destroy(gameObject); // Destruir duplicatas, se houver
        }

		//Preenchendo o vetor
		Card[] cards = Resources.LoadAll<Card>("Cards");
		allCards.AddRange(cards);
		HandManager hand = FindFirstObjectByType<HandManager>();
	}

	public List<Card> FilterCardsByType(CardType type)
	{
		return allCards.FindAll(card => card.cardType == type);
	}


	public void DrawCard(HandManager handManager)
	{
		if (GameManager.Instance.canDraw)
		{
			if (allCards.Count == 0) {
				return;
			}


			List<Card> eligibleCards = allCards.FindAll(card => card.cardType == GameManager.Instance.mainPlayer.PlayerCardType);
			currentIndex = Random.Range(0, eligibleCards.Count);
			Card nextCard = eligibleCards[currentIndex];
			handManager.addCardToHand(nextCard);

			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.CardDrew, gameObject.transform.localPosition);
			GameManager.Instance.canDraw = false;
			//Da um loop no deck caso as cartas acabem
			//currentIndex = (currentIndex + 1) % allCards.Count;
		}
	}

	//Implementar a compra de cartas de item na loja
	public void BuyCard(HandManager handManager)
	{
		if(allCards.Count == 0)
		{
			return;
		}
		currentIndex = 0;
		Card nextCard = allCards[currentIndex];
		handManager.addCardToHand(nextCard);
	}
}
