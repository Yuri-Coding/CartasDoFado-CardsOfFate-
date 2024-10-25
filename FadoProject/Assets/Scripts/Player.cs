using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int morale;
    public int influence;
    public int corruption;

    public int silenceTurn;

    // Constructor
    public Player(
        string name,
        int startingMorale,
        int startingInfluence,
        int startingCorruption)
    {
        playerName = name;
        morale = startingMorale;
        influence = startingInfluence;
        corruption = startingCorruption;

        silenceTurn = 0;
    }

    // Methods

    // Play a card from hand to field
    public void PlayCard(int handIndex)
    {
        /*
        if (handIndex >= 0 && handIndex < hand.Count)
        {
            Card cardToPlay = hand[handIndex];
            if (mana >= cardToPlay.manaCost)
            {
                mana -= cardToPlay.manaCost;
                field.Add(cardToPlay);
                hand.RemoveAt(handIndex);
                Debug.Log(playerName + " played: " + cardToPlay.name);
            }
            else
            {
                Debug.Log("Not enough mana to play " + cardToPlay.name);
            }
        }
        
        */
    }

    // Alterar parâmetro:
    // Caso queira diminuir um parâmetro, repassar valor negativo no amount.
    public void Add(string type, int amount)
    {
        switch(type)
        {
            case "morale":
                morale += amount;
                break;

            case "influence":
                influence += amount;
                break;

            case "corruption":
                corruption+= amount;
                break;
        }
    }

    // Silenciar o jogador
    public void Silence(int turn)
    {
        
    }

    // End the turn
    public void EndTurn()
    {
        // Apply any end-of-turn effects (regenerate mana, etc.)
        Debug.Log(playerName + "'s turn has ended.");
    }
}
