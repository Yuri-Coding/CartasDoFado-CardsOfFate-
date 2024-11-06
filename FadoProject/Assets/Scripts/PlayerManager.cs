using FadoProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
	public DeckManager deckManager;
	public EffectHandler effectHandler;
	public List<Player> players = new List<Player>();
	public List<Player> bots = new List<Player>();
	
	public int currentPlayerIndex = 0;

	List<Player> botEligibleTarget = new List<Player>();

	List<Player> botTarget = new List<Player>();


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

	public void ResetNotification()
	{
		foreach(Player player in players)
		{
			player.CleanNotification();
		}
	}
	// Código para Ações de NPC
	public void HandleBotAction()
	{
		foreach (Player bot in bots)
		{
			List<Card> eligibleCards = deckManager.FilterCardsByType(bot.PlayerCardType);
            int currentIndex = UnityEngine.Random.Range(0, eligibleCards.Count);
            Card botCard = eligibleCards[currentIndex];
			
			Debug.Log($"O {bot.PlayerName} usou a carta {botCard.cardName}.");
			switch (bot.PlayerRole)
			{
				case Roles.Honest:

					break;

				case Roles.Corrupt:
                case Roles.Medic:
					botTarget = new List<Player>();
					botEligibleTarget = new List<Player>();

					if (botCard.selfEffects.Count != 0) { effectHandler.ApplySelf(bot, botCard.selfEffects); }
					if (botCard.targetEffects.Count != 0) {
						
                        botEligibleTarget = players.FindAll(player => player.PlayerId != bot.PlayerId);
                        int index = UnityEngine.Random.Range(0, botEligibleTarget.Count);

                        switch (botCard.targetType)
                        {
                            // Alvo Duplo
                            case TargetType.Double:
                                botTarget.Add(botEligibleTarget[UnityEngine.Random.Range(0, botEligibleTarget.Count)]);
                                botTarget.Add(botEligibleTarget[UnityEngine.Random.Range(0, botEligibleTarget.Count)]);
                                break;

                            // Algo Single
                            case TargetType.Single:
                            case TargetType.Random:
                                // Obter um Player Aleatório e Eligível
                                botTarget.Add(botEligibleTarget[UnityEngine.Random.Range(0, botEligibleTarget.Count)]);
                                break;

                            // Obtém Metade dos Elegíveis
                            case TargetType.Half:
                                int countToSelect = botEligibleTarget.Count / 2;
                                List<Player> shuffledList = botEligibleTarget.OrderBy(x => Guid.NewGuid()).ToList();
                                botTarget = shuffledList.Take(countToSelect).ToList();
                                break;

							// Considera todos os elegíveis como alvo
                            case TargetType.All:
                                botTarget = botEligibleTarget;
                                break;

                        }

                        effectHandler.ApplyAllTargetted(botTarget, botCard.targetEffects);
					}

					/*foreach (Player target in botTarget)
					{
                        foreach (Effect bf in botCard.targetEffects)
                        {
                            Debug.Log($"[BOT] Ação de {bot.PlayerName} gerou efeito do tipo {bf.Type} para {target.PlayerName}.");
                        }
                    }*/
                    


                    break;
			}
		}
	}
}
