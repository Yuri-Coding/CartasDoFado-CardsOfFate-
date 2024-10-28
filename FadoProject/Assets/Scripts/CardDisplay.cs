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

    public Popup popup;



    // Start is called before the first frame update
    void Start()
    {
        popup = GameObject.Find("PopUp").GetComponent<Popup>();
        updateCardDisplay();
    }

    public void updateCardDisplay() {
        textName.text = cardData.cardName;
        cardImage.sprite = cardData.cardSprite;
        textEffect.text = cardData.cardEffect;
        textLore.text = cardData.cardLore;
    }

    public void updateTaskUI()
    {
        popup.PopupChoice(cardData);
    }

    public void closeTaskUI()
    {
        popup.PopdownChoice();
    }

}
