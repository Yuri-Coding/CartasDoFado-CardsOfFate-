using FadoProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{
	// Importar Handlers
	public DeckManager deckManager;
	public EffectHandler effectHandler;

	// Criar lista de Players / Bots
	public List<Player> players = new List<Player>();
	public List<Player> bots = new List<Player>();

	public List<Player> honests = new List<Player>();
	public List<Player> corrupts = new List<Player>();

	// Criar lista de targets eligíveis (tudo menos próprio player)
	public List<Player> eligibleTarget = new List<Player>();

	public int currentPlayerIndex = 0;

	List<Player> botEligibleTarget = new List<Player>();
	List<Player> botTarget = new List<Player>();

	public int poisonLimit;


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
			// Adiciona na lista BOT se é BOT
			if (player.IsBot == true) bots.Add(player);

			// Adicionar na lista de respectivos roles
			switch (player.PlayerRole)
			{
				case Roles.Honest:
				case Roles.Medic:
					honests.Add(player);
					break;
				case Roles.Corrupt:
					corrupts.Add(player);
					break;
			}

			// Adiciona no eligibleTarget se não é o próprio player
			if (player != GameManager.Instance.mainPlayer) eligibleTarget.Add(player);
			player.InitializePlayer();
		}


	}

	public void InitializeGlobalParameters()
	{
		// Veneno limite flexível para incapacitar jogador
		if		(players.Count >= 8)	{ poisonLimit = 8; }
		else if (players.Count <= 7)	{ poisonLimit = players.Count + 1; }
		else if (players.Count <= 5)	{ poisonLimit = players.Count + 2; }
		
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
		foreach (Player player in players)
		{
			player.CleanNotification();
		}
	}

	public void VerifyPoisonForAllPlayers()
	{
		foreach (Player player in players)
		{
			if (player.Poison >= poisonLimit && player.IsAlive)
			{
				KillPlayer(player);
			}
		}
	}

	public void KillPlayer(Player player)
	{
		player.IsAlive = false;

	}

	// Se nenhum corrupto estiver vivo, true
	public bool NoCorruptAlive()
	{
		foreach (Player corrupt in corrupts)
		{
			if (corrupt.IsAlive) return false;
		}
		return true;
	}

	// Se maioria dos Honestos estiverem eliminados, true
	public bool IsMostHonestEliminated()
	{
		int total = honests.Count;

		int eliminated = 0;
        foreach (Player honest in honests)
        {
            if (honest.IsAlive == false) eliminated++;
        }
		if (eliminated > total / 2) return true;
		return false;
    }

	// Código para Ações de NPC
	public void HandleBotAction()
	{
		foreach (Player bot in bots)
		{
			// Só tomar ação quando vivo
			if (!bot.IsAlive) continue;
			
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

					Debug.Log(botCard);
					Debug.Log(effectHandler);
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
