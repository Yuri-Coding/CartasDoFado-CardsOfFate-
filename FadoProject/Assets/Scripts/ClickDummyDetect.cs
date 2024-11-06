using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDummyDetect : MonoBehaviour
{
    public event Action usedCardAction;
    private void OnMouseDown()
    {
        if (GameManager.Instance.inPlay)
        {
            DummyClicked();
        }
        else
        {
            Debug.Log("Pra mirar um efeito a carta deve estar em jogo e deve ser capaz de mirar em um jogador");
        }
    }

    private void DummyClicked()
    {
        Debug.Log("O dummy foi clicado");
        usedCardAction?.Invoke();
    }
}
