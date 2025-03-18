using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using FadoProject;

using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using System.Linq;

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

    //tabela de localização
	public LocalizedStringTable OthersTable;
    public LocalizedStringTable CardsTable;


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
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Suzu, gameObject.transform.localPosition);


        itemCards = deckManager.FilterCardsByType(CardType.Item);

        influenceText.text = GameManager.Instance.mainPlayer.Influence.ToString();
        moraleText.text = GameManager.Instance.mainPlayer.Morale.ToString();

        shopObject.SetActive(true);
        UpdateShop();
        shopAnimation.Play("shop_fadein");
    }

    public void UpdateShop()
    {

        string localizeMorality = LocalizationSettings.StringDatabase.GetLocalizedString("OthersTable", "moral_store");
        string localizeInfluence = LocalizationSettings.StringDatabase.GetLocalizedString("OthersTable", "influence_store");
        string localizedCardName = "";
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
            localizedCardName = LocalizationSettings.StringDatabase.GetLocalizedString("CardsTable", productCards[i].cardName);
            productText[i].text = localizedCardName;
            productMoralePrice[i].text = $"{productCards[i].moraleCost.ToString()} {localizeMorality}";
            productInfluencePrice[i].text = $"{productCards[i].influenceCost.ToString()} {localizeInfluence}";
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
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Typewriter, gameObject.transform.localPosition);
        Debug.Log($"{productCards[index].cardName} comprada.");
        handManager.addCardToHand(productCards[index]);
        popup.UpdateSidePanel();

        CloseShop();
    }

    public void HandleCloseShopButton()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Positioning, gameObject.transform.localPosition);
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
        string localizeEffect = LocalizationSettings.StringDatabase.GetLocalizedString("CardsTable", productCards[index].cardEffect);

        return localizeEffect;
    }
}
