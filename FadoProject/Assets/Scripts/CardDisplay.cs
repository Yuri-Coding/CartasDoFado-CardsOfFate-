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

    // Start is called before the first frame update
    void Start()
    {
        updateCardDisplay();
    }

    public void updateCardDisplay() {
        textName.text = cardData.cardName;
        cardImage.sprite = cardData.cardSprite;
    }

}
