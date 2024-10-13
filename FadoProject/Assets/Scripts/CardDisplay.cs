using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FadoProject;

public class CardDisplay : MonoBehaviour
{
    public Card cardData;
    public Image cardImage;

    // Start is called before the first frame update
    void Start()
    {
        updateCardDisplay();
    }

    public void updateCardDisplay() {
        cardImage.sprite = cardData.cardSprite;
    }

}
