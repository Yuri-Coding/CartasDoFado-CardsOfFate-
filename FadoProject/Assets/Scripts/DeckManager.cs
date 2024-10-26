using FadoProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    public List<Card> allCards = new List<Card>();

    public int currentIndex = 0;

    void Start()
    {
        //Preenchendo o vetor
        Card[] cards = Resources.LoadAll<Card>("Cards");

        allCards.AddRange(cards);

        HandManager hand = FindFirstObjectByType<HandManager>();
        for(int i=0; i<1; i++)
        {
            DrawCard(hand);
        }
    }

    public void DrawCard(HandManager handManager)
    {
        if (allCards.Count == 0) {
            return;
        }
        currentIndex = Random.Range(0,allCards.Count);
        Card nextCard = allCards[currentIndex];
        handManager.addCardToHand(nextCard);
        //Da um loop no deck caso as cartas acabem
        //currentIndex = (currentIndex + 1) % allCards.Count;
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
