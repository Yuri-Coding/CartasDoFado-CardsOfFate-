using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    
    // Adicionar valor nos parâmetros do jogador
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
