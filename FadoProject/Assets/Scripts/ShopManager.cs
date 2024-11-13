using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using FadoProject;

public class ShopManager : MonoBehaviour
{
    public DeckManager deckManager;
    public HandManager handManager;

    public GameObject shopObject;
    public Animation shopAnimation;

    public List<TMP_Text> productText;
    public List<TMP_Text> productMoralePrice;
    public List<TMP_Text> productInfluencePrice;

    public List<Card> productCards;
    public List<Card> itemCards;

    public event Action closeShopAction;

    public static ShopManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Se deseja que o GameManager persista entre cenas
        }
        else
        {
            Destroy(gameObject); // Destruir duplicatas, se houver
        }
    }

    void Start()
    {
        itemCards = deckManager.FilterCardsByType(CardType.Item);
        
    }

    public void OpenShop()
    {
        shopObject.SetActive(true);
        UpdateShop();
        shopAnimation.Play("shop_fadein");
    }

    public void UpdateShop()
    {
        for (int i = 0; i < 3; i++)
        {
            Card randomItemCard;
            
            int currentIndex = UnityEngine.Random.Range(0, itemCards.Count);
            randomItemCard = itemCards[currentIndex];
            itemCards.Remove(randomItemCard);


            productCards.Add(randomItemCard);
        }
        Debug.Log($"{productCards[0].cardName}, {productCards[1].cardName}, {productCards[2].cardName}");

        for (int i = 0; i < 3; i++)
        {
            productText[i].text = productCards[i].cardName;
            productMoralePrice[i].text = $"{productCards[i].moraleCost.ToString()} Moralidade";
            productInfluencePrice[i].text = $"{productCards[i].influenceCost.ToString()} Influência";
        }
    }

    public void BuyCard(int index)
    {
        Debug.Log($"{productCards[index].cardName} comprada.");
        handManager.addCardToHand(productCards[index]);
        CloseShop();
    }

    public void CloseShop()
    {
        shopAnimation.Play("shop_fadeout");
        closeShopAction?.Invoke();
    }
}
