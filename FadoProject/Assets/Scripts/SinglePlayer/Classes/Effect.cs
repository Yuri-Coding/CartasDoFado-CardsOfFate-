using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FadoProject
{
    public enum EffectType
    {
        Silence,
        Immunity,
        AddMorale,
        AddCorruption,
        AddInfluence,
        AddPoison,
        Paralyze,
        ForceVote,
        ClearDebuff,
        SkipVote
    }

    [System.Serializable]
    public class Effect
    {
        public EffectType Type;
        public int Value;

        public Effect(EffectType type, int value)
        {
            Type = type;
            Value = value;
        }
    }
}