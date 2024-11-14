using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using FadoProject;

public class ShopManager : MonoBehaviour
{
    // Managers
    public DeckManager deckManager;
    public HandManager handManager;
    public Popup popup;

    // Object References
    public GameObject shopObject;
    public Animation shopAnimation;

    // Produto
    public List<TMP_Text> productText;
    public List<TMP_Text> productMoralePrice;
    public List<TMP_Text> productInfluencePrice;

    // Carteira
    public TMP_Text influenceText;
    public TMP_Text moraleText;

    // Cartas Produto
    public List<Card> productCards;
    public List<Card> itemCards;

    private Musics musicBeforeShop;

    // Action
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

    public void OpenShop()
    {
        musicBeforeShop = AudioManager.Instance.playingNow;
        AudioManager.Instance.SetMusic(Musics.BrilhoDeTorbernita);

        itemCards = deckManager.FilterCardsByType(CardType.Item);

        influenceText.text = GameManager.Instance.mainPlayer.Influence.ToString();
        moraleText.text = GameManager.Instance.mainPlayer.Morale.ToString();

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
            Debug.Log($"currentIndex: {currentIndex}, itemCards.Count: {itemCards.Count}");
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
        int playerInfluence = GameManager.Instance.mainPlayer.Influence;
        int playerMorale = GameManager.Instance.mainPlayer.Morale;

        if (playerMorale < productCards[index].moraleCost || playerInfluence < productCards[index].influenceCost)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Error, gameObject.transform.localPosition);
            return;
        }
        GameManager.Instance.mainPlayer.Influence -= productCards[index].influenceCost;
        GameManager.Instance.mainPlayer.Morale    -= productCards[index].moraleCost;

        Debug.Log($"{productCards[index].cardName} comprada.");
        handManager.addCardToHand(productCards[index]);
        popup.UpdateSidePanel();

        CloseShop();
    }

    public void CloseShop()
    {
        AudioManager.Instance.SetMusic(musicBeforeShop);
        shopAnimation.Play("shop_fadeout");
        closeShopAction?.Invoke();
    }

    public string GetCardInfo(int index)
    {
        return productCards[index].cardEffect;
    }
}
