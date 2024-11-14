using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string hoverInfo;
    public bool isShopCard;
    public int cardPosition;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isShopCard) {
            HoverController.Instance.ShowHover(ShopManager.Instance.GetCardInfo(cardPosition));
            return;
        }
        HoverController.Instance.ShowHover(hoverInfo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverController.Instance.HideHover();
    }
}
