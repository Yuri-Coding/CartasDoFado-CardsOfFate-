using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player
{
	public int      PlayerId { get; private set; }
	public string   PlayerName { get; set; }
	public int      Morale { get; private set; }
	public int      Influence { get; private set; }
	public int      Corruption { get; private set; }
	public int      SilenceTurn { get; set; }
    public bool		IsMainPlayer { get; set; }


    public event Action OnPlayerAction;


    // Constructor
    public Player(
		int playerId,
		string playerName,
		bool isMainPlayer)
	{
		PlayerId = playerId;
		PlayerName = playerName;
		IsMainPlayer = isMainPlayer;
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

	// Alterar par�metro:
	// Caso queira diminuir um par�metro, repassar valor negativo no amount.
	public void Add(string type, int amount)
	{
		switch(type)
		{
			case "Morale":
				Morale += amount;
				break;

			case "Influence":
				Influence += amount;
				break;

			case "Corruption":
				Corruption+= amount;
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
		Debug.Log(PlayerName + "'s turn has ended.");
	}

    public void DebugPlayer()
    {
        Debug.Log($"Player {PlayerId}: {PlayerName} - Moralidade: {Morale}, Influencia: {Influence}, Corrup��o: {Corruption}");
    }

	public void InitializePlayer()
	{
		Morale = 0;
		Influence = 0;
		Corruption = 0;
		SilenceTurn = 0;
	}
    public void PerformAction()
    {
        Debug.Log($"{PlayerName} realizou uma a��o para Player.cs.");
        OnPlayerAction?.Invoke();
    }
}