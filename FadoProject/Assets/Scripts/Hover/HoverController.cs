using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoverController : MonoBehaviour
{
    public static HoverController Instance; // Singleton para acesso global

    [SerializeField] private GameObject hoverWindow; // Janela de informações
    [SerializeField] private TMP_Text hoverText;     // Texto exibido na janela
    [SerializeField] private Vector3 offset;         // Offset para posicionar a janela

    private RectTransform canvasRect; // Referência ao Canvas principal
    private bool isHovering = false; // Adicionado para evitar loops de SetActive

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (hoverWindow != null)
        {
            hoverWindow.SetActive(false); // Esconde a janela inicialmente
        }

        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            canvasRect = parentCanvas.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        if (hoverWindow.activeSelf && isHovering)
        {
            Vector2 mousePosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePosition, null, out Vector2 localPoint);
            hoverWindow.GetComponent<RectTransform>().anchoredPosition = localPoint + (Vector2)offset;
        }
    }

    public void ShowHover(string info)
    {
        if (!isHovering)
        {
            isHovering = true;
            hoverText.text = info;
            hoverWindow.SetActive(true);
        }
    }

    public void HideHover()
    {
        if (isHovering)
        {
            isHovering = false;
            hoverWindow.SetActive(false);
        }
    }
}