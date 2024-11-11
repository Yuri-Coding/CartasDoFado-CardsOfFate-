using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FadoProject;

public class EffectHandler : MonoBehaviour
{
	public Player mainPlayer;
	public Popup popup;

	public void ApplySelf(Player player, List<Effect> effects)
	{
		foreach (Effect ef in effects)
		{
			ApplyValue(player, ef.Type, ef.Value);
		}

		popup.UpdatePanel();
	}

    public void ApplyMain(List<Effect> effects)
    {
        mainPlayer = GameManager.Instance.mainPlayer;
        foreach (Effect ef in effects)
        {
            //Debug.Log(ef.Type);
            ApplyValue(mainPlayer, ef.Type, ef.Value);
        }

        popup.UpdatePanel();
        PlayerActionDone();
    }

	public void ApplyAllTargetted(List<Player> tplayers, List<Effect> teffects)
	{
		foreach (Player tplayer in tplayers)
		{
			foreach(Effect teffect in teffects)
			{
                switch (teffect.Type)
                {
                    case EffectType.Silence:
                        tplayer.ApplySilence(teffect.Value);
                        break;

                    case EffectType.Immunity:
                        tplayer.ApplyImmunity(teffect.Value);
                        break;

                    case EffectType.AddMorale:
                    case EffectType.AddCorruption:
                    case EffectType.AddInfluence:
					case EffectType.AddPoison:
                        ApplyValue(tplayer, teffect.Type, teffect.Value);
                        break;

					case EffectType.ForceVote:

						break;

					case EffectType.Paralyze:

						break;

					case EffectType.ClearDebuff:

						break;
                }
            }
		}
	}


	public void ApplyValue (Player target, EffectType type,  int amount) 
	{
		switch (type)
		{
			case EffectType.AddMorale:
				target.Add("Morale", amount);
				break;

			case EffectType.AddInfluence:
				target.Add("Influence", amount);
				break;

			case EffectType.AddCorruption:
				target.Add("Corruption", amount);
				break;
			case EffectType.AddPoison:
				target.Add("Poison", amount);
				break;
		}
	}

	public void PlayerActionDone()
	{
		// Chamar evento para falar que o jogador executou uma a鈬o.
		mainPlayer.PerformAction();
	}

	public void ApplySilence (Player target, int duration)
	{
		target.ApplySilence(duration);
	}

	public void ApplyImmunity (Player target, int duration)
	{
		target.ApplyImmunity(duration);
	}

	public void ApplyClearDebuff(Player target)
	{
		
	}
}
