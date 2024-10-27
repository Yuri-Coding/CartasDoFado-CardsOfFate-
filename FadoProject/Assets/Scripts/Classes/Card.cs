using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FadoProject {

    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class Card : ScriptableObject{
        public string cardName;
        public Sprite cardSprite;
        public Sprite zoomedSprite;
        public CardType cardType;
        public string cardEffect;
        public string cardLore;
        public List<Effects> effects;
        public bool canTarget;
        public string choice1;
        public string choice2;

        public enum CardType
        {
            Task,
            Poison,
            Medicine,
            Item
        }

        public enum Effects
        {
            Silence,
            Immunity,
            PlusMorale,
            MinusMorale,
            PlusInfluence,
            MinusInfluence,
            PlusPoison,
            MinusPoison,
            PlusCorruption,
            MinusCorruption
        }
    }

}