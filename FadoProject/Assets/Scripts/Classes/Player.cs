using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player
{

	// Informação do Jogador
	public int		PlayerId { get; private set; }
	public string	PlayerName { get; set; }
	public Roles	PlayerRole { get; set; }
	public CardType PlayerCardType { get; set; }



	// Parâmetros
	public int		Morale        { get; set; }
	public int		Influence     { get; set; }
	public int		Poison        { get; set; }
	public int		Corruption    { get; set; }
    public int		votesReceived { get; set; }



    // Efeitos
    public int		SilenceTurn   { get; private set; }
	public int		ImmunityTurn  { get; private set; }
    public int		ForceVoteTurn { get; private set; }
    public int		ParalyzeTurn  { get; private set; }
	public int		SkipVoteTurn  { get; private set; }



	// Indicadores
	public bool		IsMainPlayer { get; set; }
    public bool		IsBot { get; set; }
	public bool		IsAlive { get; set; }
	public Player	vote { get; private set; }



	public List<Notification> notifications;


    public event Action OnPlayerAction;


	// Constructor
	public Player(
		int		playerId,
		string	playerName,
		Roles	playerRole,
		bool	isMainPlayer,
		bool	isBot,
		bool	isAlive)
	{
		PlayerId =			playerId;
		PlayerName =		playerName;
		PlayerRole =		playerRole;
		PlayerCardType =	GetCardType(PlayerRole);
		IsMainPlayer =		isMainPlayer;
		IsBot =				isBot;
		IsAlive =			isAlive;
	}
	public void PlayCard(int handIndex)
	{
		
	}

	// Alterar parâmetro:
	// Caso queira diminuir um par穃etro, repassar valor negativo no amount.
	public void Add(string type, int amount)
	{
		switch (type)
		{
			case "Morale":
				Morale += amount;
				if (Morale < 0) Morale = 0;
                //notifications.Add(new Notification(FadoProject.EffectType.AddMorale, amount));
                break;

			case "Influence":
				Influence += amount;
				if(Influence < 0) Influence = 0;
                //notifications.Add(new Notification(FadoProject.EffectType.AddInfluence, amount));
                break;

			case "Corruption":
                Corruption += amount;
				if(Corruption < 0) Corruption = 0;
                notifications.Add(new Notification(FadoProject.EffectType.AddCorruption, amount));
                break;
			case "Poison":
				if (ImmunityTurn > 0) return;
                Poison += amount;
                if (Poison < 0) Poison = 0;
                notifications.Add(new Notification(FadoProject.EffectType.AddPoison, amount));
                break;
		}
    }


	// Silenciar o jogador
	public void ApplySilence(int duration)
	{
		if (ImmunityTurn > 0) return;

        notifications.Add(new Notification(FadoProject.EffectType.Silence, duration));
		SilenceTurn += duration;
	}

	public void ApplyImmunity(int duration)
	{
		ImmunityTurn += duration;
	}

	public void ApplyParalyze(int duration)
	{
        if (ImmunityTurn > 0) return;

        notifications.Add(new Notification(FadoProject.EffectType.Paralyze, duration));
        ParalyzeTurn += duration;
    }

	public void ApplyForceVote(int duration)
	{
        if (ImmunityTurn > 0) return;

        notifications.Add(new Notification(FadoProject.EffectType.ForceVote, duration));
        ForceVoteTurn += duration;
    }

	public void ApplyClearDebuff()
	{
		SilenceTurn  = 0;
		ParalyzeTurn = 0;
		SkipVoteTurn = 0;
	}

	// End the turn
	public void EndTurn()
	{
		if (SilenceTurn  > 0) SilenceTurn  -= 1;
        if (ImmunityTurn > 0) ImmunityTurn -= 1;
        if (ParalyzeTurn > 0) ParalyzeTurn -= 1;
        if (SkipVoteTurn > 0) SkipVoteTurn -= 1;
	}

	public void DebugPlayer()
	{
		Debug.Log($"Player {PlayerId}: {PlayerName} - Moralidade: {Morale}, Influencia: {Influence}, Corrupção: {Corruption}");
	}

	public void InitializePlayer()
	{
		Morale = 0;
		Influence = 0;
		Corruption = 0;
        Poison = 0;
        SilenceTurn = 0;
		votesReceived = 0;

		notifications = new List<Notification>();
	}
	public void PerformAction()
	{
		//Debug.Log($"{PlayerName} realizou uma ação para Player.cs.");
		OnPlayerAction?.Invoke();
	}

    public CardType GetCardType(Roles role)
    {
        switch (role)
        {
            case Roles.Honest:
                return CardType.Task;

            case Roles.Corrupt:
                return CardType.Poison;

            case Roles.Medic:
                return CardType.Medic;
        }
		Debug.LogError("Não foi selecionado.");
		return CardType.Medic;
    }

	public void CheckoutAllNotifications()
	{
		foreach(Notification notification in notifications)
		{
			notification.CheckoutNotification();
		}
	}

	public void CleanNotification()
	{
		//Debug.Log($"CleanNotification() para {PlayerName}");
		notifications = new List<Notification>();
	}
}
