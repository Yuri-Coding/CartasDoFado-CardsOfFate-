using FadoProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public DeckManager deckManager;
	public List<Player> players = new List<Player>();
	public List<Player> bots = new List<Player>();
	
	public int currentPlayerIndex = 0;

	public void AddPlayer(Player newPlayer) {
		players.Add(newPlayer);
	}

	public void RemovePlayer(Player playerToRemove) {
		players.Remove(playerToRemove);
	}

	public Player GetPlayerById(int playerId) {
		return players.Find(p => p.PlayerId == playerId);
	}

	public List<Player> GetAllPlayers() {
		return players;
	}

	public void InitializePlayers()
	{
		foreach (Player player in players)
		{
			if (player.IsBot == true) { bots.Add(player); }
			player.InitializePlayer();
		}
	}

	public bool IsMainPlayer(Player player)
	{
		return player.IsMainPlayer;
	}

	public void DebugAction()
	{
        players[currentPlayerIndex].PerformAction();
		Debug.Log("Ação acionada em Player.");
	}

	public void HandleBotAction()
	{
		foreach (Player bot in bots)
		{
			List<Card> eligibleCards = deckManager.FilterCardsByType(bot.PlayerCardType);
            int currentIndex = Random.Range(0, eligibleCards.Count);
            Card botCard = eligibleCards[currentIndex];

			Debug.Log($"O {bot.PlayerName} usou a carta {botCard.cardName}.");
        }
	}
}
