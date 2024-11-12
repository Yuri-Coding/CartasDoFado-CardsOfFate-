using FadoProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManagerMP : MonoBehaviour
{
	public List<Card> allCards = new List<Card>();

	public int currentIndex = 0;

	void Start()
	{
		//Preenchendo o vetor
		Card[] cards = Resources.LoadAll<Card>("Cards");

		allCards.AddRange(cards);

		HandManagerMP hand = FindFirstObjectByType<HandManagerMP>();
	}

	public List<Card> FilterCardsByType(CardType type)
	{
		return allCards.FindAll(card => card.cardType == type);
	}


	public void DrawCard(HandManagerMP handManager)
	{
		if (GameManagerMP.Instance.canDraw)
		{
			if (allCards.Count == 0) {
				return;
			}


			List<Card> eligibleCards = allCards.FindAll(card => card.cardType == GameManagerMP.Instance.mainPlayer.PlayerCardType);
			currentIndex = Random.Range(0, eligibleCards.Count);
			Card nextCard = eligibleCards[currentIndex];
			handManager.addCardToHand(nextCard);

			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.CardDrew, gameObject.transform.localPosition);
			GameManagerMP.Instance.canDraw = false;
			//Da um loop no deck caso as cartas acabem
			//currentIndex = (currentIndex + 1) % allCards.Count;
		}
	}

	//Implementar a compra de cartas de item na loja
	public void BuyCard(HandManagerMP handManager)
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
