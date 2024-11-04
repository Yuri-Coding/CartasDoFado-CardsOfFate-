using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public List<Player> players = new List<Player>();
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

	public Player GetNextPlayer()
	{
		currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
		return players[currentPlayerIndex];
	}

	public void InitializePlayers()
	{
		foreach (Player player in players)
		{
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
}
