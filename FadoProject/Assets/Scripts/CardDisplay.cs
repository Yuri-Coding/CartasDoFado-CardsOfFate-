using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FadoProject;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card cardData;
    public Image cardImage;
    public TMP_Text textName;
    public TMP_Text textEffect;
    public TMP_Text textLore;

    // Start is called before the first frame update
    void Start()
    {
        updateCardDisplay();
    }

    public void updateCardDisplay() {
        textName.text = cardData.cardName;
        cardImage.sprite = cardData.cardSprite;
        textEffect.text = cardData.cardEffect;
        textLore.text = cardData.cardLore;
    }

}
