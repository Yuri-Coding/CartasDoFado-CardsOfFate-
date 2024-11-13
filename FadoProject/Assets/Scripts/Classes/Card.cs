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

        public bool canTarget;
        public TargetType targetType;

        public string choice1;
        public string choice2;

        public int moraleCost;
        public string influenceCost;

        public List<Effect> targetEffects;
        public List<Effect> selfEffects;
        public List<Effect> choice1Effects;
        public List<Effect> choice2Effects;
    }
}