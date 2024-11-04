using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FadoProject;

public class EffectHandler : MonoBehaviour
{
    public Player mainPlayer;
    public Popup popup;

    public void ApplySelf(List<Effect> effects)
    {
        mainPlayer = GameManager.Instance.mainPlayer;
        Debug.Log(mainPlayer.PlayerName);
        foreach (Effect ef in effects)
        {
            Debug.Log(ef.Type);
            ApplyValue(mainPlayer, ef.Type, ef.Value);
        }

        popup.UpdatePanel();
        PlayerActionDone();
    }

    public void ApplySingleTarget (Player tplayer, List<Effect> selfEffects, List<Effect> targetEffects)
    {
        foreach (Effect ef in targetEffects)
        {
            ApplyValue(tplayer, ef.Type, ef.Value);
        }
        ApplySelf(selfEffects);
    }

    public void ApplyMultipleTarget(List<Player> tplayers, List<Effect> selfEffects, List<Effect> targetEffects)
    {
        foreach (Player player in tplayers)
        {
            foreach (Effect ef in targetEffects)
            {
                ApplyValue(player, ef.Type, ef.Value);
            }
        }
        ApplySelf(selfEffects);
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
        }
    }

    public void PlayerActionDone()
    {
        // Chamar evento para falar que o jogador executou uma ação.
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
}
