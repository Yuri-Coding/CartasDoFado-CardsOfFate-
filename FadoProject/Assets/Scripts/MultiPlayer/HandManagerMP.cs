﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FadoProject;
using System;

public class HandManagerMP : MonoBehaviour
{
    public DeckManagerMP DeckManager;

    public GameObject cardPrefab; //Serve pra colocar o prefab de carta criado, usa o inspector

    public Transform handTransform; //Define uma "âncora" pra posição da mão

    public float HandSpread = -5f;//Vai servir pra angular as cartas na mão

    public List<GameObject> cardsInHand = new List<GameObject>(); //Lista de cartas(o objeto) que est?o na m?o

    public float HorizontalSpacing = 175f;
    public float VerticalSpacing = 50f;

    private void Update()
    {
        //updateHandVisuals();
    }

    //Adiciona uma carta na mão(apenas os dados)
    public void addCardToHand(Card cardData)
    {
        //Instanciando a carta
        GameObject newCard = Instantiate(cardPrefab, handTransform.position, Quaternion.identity, handTransform);
        cardsInHand.Add(newCard);

        //Colocar os dados da carta na carta instanciada
        newCard.GetComponent<CardDisplayMP>().cardData = cardData;
        newCard.GetComponent<CardMovementMP>().currentCard = cardData;

        updateHandVisuals();
    }
    
    //Adiciona uma carta na mão (visualmente)
    public void updateHandVisuals()
    {
        int cardCount = cardsInHand.Count;
        for (int i = 0; i < cardCount; i++)
        {
            //Serve pra tirar um erro de divis?o por 0
            if(cardCount == 1)
            {
                cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                cardsInHand[i].transform.localPosition = new Vector3(0f, 0f, 0f);
                return;
            }

            //Angulando as cartas na m?o
            float cardAngle = (HandSpread * (i - (cardCount - 1) / 2f));
            cardsInHand[i].transform.localRotation = Quaternion.Euler(0f, 0f, cardAngle);
            
            //Arrumando a posi??o horizontal com o centro da mão
            float horizontalSpread = (HorizontalSpacing * (i - (cardCount - 1) / 2f));

            //Arrumando a posi??o vertical com o centro da mão
            float normalizedPosition = (2f * i / (cardCount - 1) - 1f);
            float verticalSpread = VerticalSpacing * (1 - normalizedPosition * normalizedPosition);

            //Aplicando as posi??es
            cardsInHand[i].transform.localPosition = new Vector3(horizontalSpread, verticalSpread, 0f);
        }
    }
}
