using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    public Player currentPlayer;

    public void ApplyTask(List<Effect> effects)
    {
        currentPlayer = GameManager.currentPlayer;
        Debug.Log(currentPlayer.PlayerName);
        foreach (Effect ef in effects)
        {
            Debug.Log(ef.Type);
        }
    }

    public void ApplyAll (Player cplayer, Player tplayer, List<Effect> effects)
    {
        currentPlayer = cplayer;
        foreach (Effect ef in effects)
        {

        }
    }

    public void ApplyValue (Player target, string type,  int amount) 
    {
        switch (type)
        {
            case "morale":
                target.Add("Morale", amount);
                break;

            case "influence":
                target.Add("Influence", amount);
                break;

            case "corruption":
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
