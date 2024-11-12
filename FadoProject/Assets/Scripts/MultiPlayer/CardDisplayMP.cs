using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FadoProject;
using TMPro;
using System;

public class CardDisplayMP : MonoBehaviour
{
    public Card cardData;
    public Image cardImage;
    public TMP_Text textName;
    public TMP_Text textEffect;
    public TMP_Text textLore;

    public PopupMP popup;
    private Dictionary<CardType, Color> cardTypeColors;


    // Start is called before the first frame update
    private void Awake()
    {
        cardTypeColors = new Dictionary<CardType, Color>()
        {
            {CardType.Medic, new Color(0.0f, 0.561f, 0.196f) },
            {CardType.Poison, new Color(0.957f, 0.263f, 0.212f) },
            {CardType.Task, new Color(0.0f, 0.286f, 0.541f) },
            {CardType.Item, new Color(0,0,0) }//Definir ainda
        };
    }

    void Start()
    {
        popup = GameObject.Find("PopupManager").GetComponent<PopupMP>();
        updateCardDisplay();
        SetCardTextColor();
    }

    private void SetCardTextColor()
    {
        if(cardTypeColors.TryGetValue(cardData.cardType, out Color color))
        {
            textName.color = color;
        }
        else
        {
            Debug.Log("Categoria de cartas não tem cor definida");
        }
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
