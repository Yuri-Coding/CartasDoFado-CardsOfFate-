using FadoProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using System.Linq;

public class Notification
{
	//tabelas de localização
	public LocalizedStringTable NotificationTable;

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
				FinalText = AddCorruptText(Amount);
				break;

			case EffectType.AddPoison:
				FinalText = AddPoisonText(Amount);
				break;

			case EffectType.AddInfluence:
				FinalText = AddText("influência", Amount);
				break;

			case EffectType.Paralyze:
				FinalText = ParalyzeText(Amount);
				break;

			case EffectType.SkipVote:
				FinalText = SkipVoteText(Amount);
				break;

			case EffectType.ClearDebuff:
				FinalText = ClearDebuffText();
				break;
		}
	}

	private string SilenceText(int turn)
	{
		// List<string> possibleText = new List<string>
		// {
		// 	"Oh não! Alguém te silenciou por {0} turno{1}...",
		// 	"Sintomas surgiram,as suas vozes foram silenciadas por {0} turno{1}...",
		// 	"Parece que alguém não gostou do que você expôs. Você foi silenciado por {0} turno{1}."
		// };
		// int index = UnityEngine.Random.Range(0, possibleText.Count);

		// Lista de chaves das mensagens disponíveis
		List<string> possibleKeys = new List<string>
		{
			"notification_silence_1",
			"notification_silence_2",
			"notification_silence_3"
		};

		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];
		string suffix = "";
		if (turn > 1) suffix = "s";

		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return String.Format(localizedText, turn, suffix);
		// return String.Format(possibleText[index], turn, suffix);
	}

	private string ImmunityText(int turn)
	{
		// List<string> possibleText = new List<string>
		// {
		// 	"O Médico lhe concedeu uma proteção medicinal por {0} turno{1}.",
		// };
		// int index = UnityEngine.Random.Range(0, possibleText.Count);


		List<string> possibleKeys = new List<string>
		{
			"notification_imunity_1",
		};

		string suffix = "";
		if (turn > 1) suffix = "s";

		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];

		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return string.Format(localizedText, turn, suffix);

		// return String.Format(possibleText[index], turn, suffix);
	}

	private string ForceVoteText()
	{
		List<string> possibleKeys = new List<string>
		{
			"notification_force_vote_1",
		};

		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];

		// int index = UnityEngine.Random.Range(0, possibleText.Count);

		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return localizedText;
	}

	private string AddText(string parameter, int amount)
	{
		// List<string> possibleText;
		// if (amount > 0)
		// {
		// 	possibleText = new List<string>
		// 	{
		// 		"Alguém te ajudou, e adicionou {0} de {1}" +
		// 		"Ora, parece que você recebeu os benefícios de alguém, resultando no aumento de {1} por {0}.",
		// 	};
		// }
		// else
		// {
		// 	possibleText = new List<string>
		// 	{
		// 		"Parece que alguém não está gostando de você... Alguém tirou {0} de {1}.",
		// 		"Alguém está espalhando mentiras sobre você... {0} a menos de {1}."
		// 	};
		// }

		List<string> possibleKeys;
		if (amount > 0)
		{
			possibleKeys = new List<string>
			{
				"notification_add_positive_1",
				"notification_add_positive_2",
			};
		}
		else
		{
			possibleKeys = new List<string>
			{
				"notification_add_negative_1",
				"notification_add_negative_2",
			};
		}

		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];
		// int index = UnityEngine.Random.Range(0, possibleText.Count);
		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return String.Format(localizedText, amount, parameter);
	}

	private string AddPoisonText(int amount)
	{
		// List<string> possibleText;
		// if (amount > 0)
		// {
		// 	possibleText = new List<string>
		// 	{
		// 		"Infelizmente, nem todas as pessoas gostam de você, e aplicou {0} de veneno...",
		// 		"Você acordou com uma sensação estranha, o que fez perceber que alguém o envenenou. {0} de veneno.",
		// 		"Você percebe que sua visão está turva, e então você descobre que foi envenenado por {0} de veneno.",
		// 		"As mãos malígnas do corrupto deixou claros rastros de destruição. Foi envenenado por {0} de veneno."
		// 	};
		// }
		// else
		// {
		// 	possibleText = new List<string>
		// 	{
		// 		"O médico fez um ótimo trabalho e conseguiu desintoxicar {0} de veneno de você.",
		// 		"O médico mostrou a sua habilidade surpreendente, e retirou do seu corpo {0} de veneno. "
		// 	};
		// }

		List<string> possibleKeys;
		if (amount > 0)
		{
			possibleKeys = new List<string>
			{
				"notification_add_poison_1",
				"notification_add_poison_2",
				"notification_add_poison_3",
				"notification_add_poison_4",
			};
		}
		else
		{
			possibleKeys = new List<string>
			{
				"notification_remove_poison_1",
				"notification_remove_poison_2",
			};
		}

		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];

		// int index = UnityEngine.Random.Range(0, possibleText.Count);
		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return String.Format(localizedText, amount);
	}

	private string AddCorruptText(int amount)
	{
		// List<string> possibleText;
		// if (amount > 0)
		// {
		// 	possibleText = new List<string>
		// 	{
		// 		"Ah não, alguém o corrompeu! A corrupção o corrompeu em {0}..."
		// 	};
		// }
		// else
		// {
		// 	possibleText = new List<string>
		// 	{
		// 		"Alguém salvou você, o seu nível de corrupção diminuiu em {0}.",
		// 	};
		// }

		List<string> possibleKeys;
		if (amount > 0)
		{
			possibleKeys = new List<string>
			{
				"notification_add_corrupt_1",
			};
		}
		else
		{
			possibleKeys = new List<string>
			{
				"notification_remove_corrupt_1",
			};
		}


		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];

		// int index = UnityEngine.Random.Range(0, possibleText.Count);
		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return String.Format(localizedText, amount);
	}

	private string ParalyzeText(int turn)
	{
		// List<string> possibleText = new List<string>
		// {
		// 	"O poder maligno da corrupção impossibilitou de você tomar ações por {0} turno{1}.",
		// 	"A manhã foi aterrorizante, marcado por movimentações limitados do seu corpo. Você foi impossibilitado em tomar ações por {0} turno{1}.",
		// 	"o seu corpo não está obedecendo as suas vontades. Parece que alguém o paralizou por {0} turno{1}."
		// };

		List<string> possibleKeys = new List<string>
		{
			"notification_paralyze_1",
			"notification_paralyze_2",
			"notification_paralyze_3",
		};
		// int index = UnityEngine.Random.Range(0, possibleText.Count);
		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];
		string suffix = "";
		if (turn > 1) suffix = "s";

		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return string.Format(localizedText, turn, suffix);

		// return String.Format(possibleText[index], turn, suffix);
	}

	private string SkipVoteText(int turn)
	{
		// List<string> possibleText = new List<string>
		// {
		// 	"Alguém não está gostando das suas escolhas e impossibilitou de você votar por {0} turno{1}.",
		// 	"Infelizmente, alguém retirou o seu direito de votar por {0} turno{1}."
		// };

		List<string> possibleKeys = new List<string>
		{
			"notification_skip_vote_1",
			"notification_skip_vote_2",
		};

		// int index = UnityEngine.Random.Range(0, possibleText.Count);
		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];
		string suffix = "";
		if (turn > 1) suffix = "s";

		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return string.Format(localizedText, turn, suffix);

		// return String.Format(possibleText[index], turn, suffix);
	}

	private string ClearDebuffText()
	{
		List<string> possibleKeys = new List<string>
		{
			"notification_clear_debuff_1",
			"notification_clear_debuff_2",
		};
		// int index = UnityEngine.Random.Range(0, possibleText.Count);
		int index = UnityEngine.Random.Range(0, possibleKeys.Count);
		string selectedKey = possibleKeys[index];

		string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("NotificationsTable", selectedKey);

		return localizedText;
		// return possibleText[index];
	}
}
