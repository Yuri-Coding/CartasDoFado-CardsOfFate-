using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FadoProject;

public class EffectHandler : MonoBehaviour
{
    public Player currentPlayer;
    public Popup popup;

    public void ApplySelf(List<Effect> effects)
    {
        currentPlayer = GameManager.currentPlayer;
        Debug.Log(currentPlayer.PlayerName);
        foreach (Effect ef in effects)
        {
            Debug.Log(ef.Type);
            ApplyValue(currentPlayer, ef.Type, ef.Value);
        }
        popup.UpdatePanel();
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

    public void ApplySilence (Player target, int duration)
    {
        target.ApplySilence(duration);
    }

    public void ApplyImmunity (Player target, int duration)
    {
        target.ApplyImmunity(duration);
    }
}
