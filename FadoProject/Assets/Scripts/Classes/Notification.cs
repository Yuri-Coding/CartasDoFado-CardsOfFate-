using FadoProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification
{
	public EffectType EffectType { get; private set; }
	public int Amount { get; private set; }
	public string FinalText { get; private set; }

	public Notification(EffectType effectType, int amount)
	{
		EffectType = effectType;
		Amount = amount;
	}

	public void CheckoutNotification()
	{
		switch (EffectType)
		{
			case EffectType.Silence:
				FinalText = SilenceText(Amount);
				break;

			case EffectType.Immunity:
				FinalText = ImmunityText(Amount);
				break;

			case EffectType.ForceVote:
				FinalText = ForceVoteText();
				break;

			case EffectType.AddMorale:
				FinalText = AddText("moral", Amount);
				break;

			case EffectType.AddCorruption:
				FinalText = AddText("corrupção", Amount);
				break;

			case EffectType.AddInfluence:
				FinalText = AddText("influência", Amount);
				break;

			case EffectType.ActionReduction:

				break;
		}
	}

	private string SilenceText(int turn)
	{
		List<string> possibleText = new List<string>
		{
			"Oh não! Alguém te silenciou por {0} turno{1}...",
			"Sintomas surgiram,as suas vozes foram silenciadas por {0} turno{1}...",
			"Parece que alguém não gostou do que você expôs. Você foi silenciado por {0} turno{1}."
		};
		int index = UnityEngine.Random.Range(0, possibleText.Count);
		
		string suffix = "";
		if (turn > 1) suffix = "s"; 

		return String.Format(possibleText[index], turn, suffix);
	}

	private string ImmunityText(int turn)
	{
		List<string> possibleText = new List<string>
		{
			"O Médico lhe concedeu uma proteção medicinal por {0} turno{1}.",
		};
		int index = UnityEngine.Random.Range(0, possibleText.Count);

		string suffix = "";
		if (turn > 1) suffix = "s";

		return String.Format(possibleText[index], turn, suffix);
	}

	private string ForceVoteText()
	{
		List<string> possibleText = new List<string>
		{
			"O seu voto está diferente do que votou...? Alguém manipulou os seus votos!",
		};
		int index = UnityEngine.Random.Range(0, possibleText.Count);
		return possibleText[index];
	}

	private string AddText(string parameter, int amount)
	{
		List<string> possibleText;
		if (amount > 0)
		{
			possibleText = new List<string>
			{
				"Alguém te ajudou, e adicionou {0} de {1}" +
				"Ora, parece que você recebeu os benefícios de alguém, resultando no aumento de {1} por {0}.",
			};
		} else {
			possibleText = new List<string>
			{
				"Parece que alguém não está gostando de você... Alguém tirou {0} de {1}.",
				"Alguém está espalhando mentiras sobre você... {0} a menos de {1}."
			};
		}
			

		
		int index = UnityEngine.Random.Range(0, possibleText.Count);

		return String.Format(possibleText[index], amount, parameter);
	}

	private string AddCorruptText(int amount)
    {
        List<string> possibleText;
        if (amount > 0)
        {
            possibleText = new List<string>
			{
				"Ah não, alguém o corrompeu! A corrupção o corrompeu em {0}..."
            };
        }
        else
        {
            possibleText = new List<string>
            {
                "Alguém salvou você, o seu nível de corrupção diminuiu em {0}.",
            };
        }



        int index = UnityEngine.Random.Range(0, possibleText.Count);

        return String.Format(possibleText[index], amount);
    }
}
