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

    public TMP_Text choiceContext;
    public TMP_Text choiceText1;
    public TMP_Text choiceText2;

    public Animation anim;



    // Start is called before the first frame update
    void Start()
    {
        choiceContext = GameObject.Find("ChoiceContext").GetComponent<TMP_Text>();
        choiceText1 = GameObject.Find("Choice1Text").GetComponent<TMP_Text>();
        choiceText2 = GameObject.Find("Choice2Text").GetComponent<TMP_Text>();

        anim = GameObject.Find("ChoicePopup").GetComponent<Animation>();
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
        anim.Play("fadein");
        choiceContext.text = cardData.cardLore;
        choiceText1.text = cardData.choice1;
        choiceText2.text = cardData.choice2;
    }

    public void closeTaskUI()
    {
        anim.Play("fadeout");
    }

}
