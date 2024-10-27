using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;

    private GameObject canvasObject;
    private RectTransform newCanvas;
    private GameObject newCanvasObject;//N tem nenhuma relação com canvas kkk, tá pegando o handPos só msm mas deu preguiça de mudar o nome
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private int currentState = 0;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private Vector3 cardPlay;
    [SerializeField] private Vector3 playPosition;
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;
    [SerializeField] private float lerpFactor = 0.1f;

    //Coisas das text boxes
    [SerializeField] private GameObject effectBox;
    [SerializeField] private GameObject loreBox;

    //Vars para dar zoom
    [SerializeField] private float zoomFactor = 2.0f;
    private Vector3 zoomedPosition;
    private float zoomDuration = 0.2f;
    private bool isZoomed = false;



    void Awake() 
    {
        newCanvasObject = GameObject.Find("HandPos");
        newCanvas = newCanvasObject.GetComponent<RectTransform>();
        //newCanvas.anchoredPosition = new Vector3(1000,1000,1); Da uma testadinha aqui
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
        glowEffect.SetActive(false);
        playArrow.SetActive(false);

    }

    void Update()
    {
        switch (currentState)
        {
            case 1:
                HandleHoverState();
                break;
            case 2:
                HandleDragState();
                if (!Input.GetMouseButton(0)) 
                {
                    TransitionToState0();
                }
                break;
            case 3:
                HandlePlayState();
                /*if (!Input.GetMouseButton(0))
                {
                    TransitionToState0();
                }*/
                break;
            case 4:
                if (isZoomed == true) return;
                StartCoroutine(ZoomIn());
                break;
            case 5:
                if (isZoomed == false) return;
                StartCoroutine(ZoomOut());
                break;
            case 6:
                //Idle Zoom
                break;
        }
    }

    //Volta a carta pro estado original dela (em rotação, escala e posição)
    private void TransitionToState0()
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
        glowEffect.SetActive(false);
        playArrow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        if(currentState == 0)
        {
            originalPosition = rectTransform.localPosition;
            originalRotation = rectTransform.localRotation;
            originalScale = rectTransform.localScale;

            currentState = 1;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(currentState == 1 && eventData.button == PointerEventData.InputButton.Left)
        {
            currentState = 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            originalPanelLocalPosition = rectTransform.localPosition;
        }
        if(currentState == 1 && eventData.button == PointerEventData.InputButton.Right)
        {
            currentState = 4;
        }
        if(currentState == 6 && eventData.button == PointerEventData.InputButton.Right)
        {
            currentState = 5;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == 2)
        {
            Vector2 localPointerPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localPointerPosition))
            {
                rectTransform.position = Vector3.Lerp(rectTransform.position,Input.mousePosition, lerpFactor);

                if (rectTransform.localPosition.y > cardPlay.y)
                {
                    currentState = 3;
                    playArrow.SetActive(true);
                    rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition,playPosition,lerpFactor);
                }
            }
        }
    }

    private void HandleHoverState() 
    {
        glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;
    }

    private void HandleDragState()
    {
        rectTransform.localRotation = Quaternion.identity;//Zerando a rotação da carta
    }

    private void HandlePlayState()
    {
        rectTransform.localPosition = playPosition;
        rectTransform.localRotation = Quaternion.identity;

        if(Input.mousePosition.y < cardPlay.y)
        {
            currentState = 2;
            playArrow.SetActive(false);
        }
    }

    //Achando o centro da tela

    private Vector3 GetScreenCenterPosition()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height /2, 0f);

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenCenter, canvas.worldCamera, out localPoint);
        return localPoint;
    }


    //Funções de zoom

    private System.Collections.IEnumerator ZoomIn()
    {
        isZoomed = true;
        float elapsedTime = 0f;
        zoomedPosition = GetScreenCenterPosition();
        zoomedPosition.x += -250;
        zoomedPosition.y += 700;
        glowEffect.SetActive(false);

        while (elapsedTime < zoomDuration)
        {
            rectTransform.localPosition = Vector3.Lerp(originalPosition, zoomedPosition, elapsedTime / zoomDuration);
            rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * zoomFactor, elapsedTime / zoomDuration);
            rectTransform.localRotation = Quaternion.Lerp(originalRotation, Quaternion.identity, elapsedTime / zoomDuration);
            newCanvas.anchoredPosition = Vector3.Lerp(new Vector3(-100,-315,0), new Vector3 (-100, -500, 0), elapsedTime / zoomDuration);
            elapsedTime = elapsedTime + Time.deltaTime;
            yield return null;
        }
        //Colocar ativação de text boxes
        effectBox.SetActive(true);
        loreBox.SetActive(true);

        /*transform.position = zoomedPosition;
        transform.localScale = originalScale * zoomFactor;
        transform.rotation = Quaternion.identity;*/
        currentState = 6;
    }

    private System.Collections.IEnumerator ZoomOut()
    {
        isZoomed = false;
        float elapsedTime = 0f;
        zoomedPosition = GetScreenCenterPosition();
        glowEffect.SetActive(false);

        //Desativando text boxes
        effectBox.SetActive(false);
        loreBox.SetActive(false);

        while (elapsedTime < zoomDuration)
        {
            rectTransform.localPosition = Vector3.Lerp(zoomedPosition, originalPosition, elapsedTime / zoomDuration);
            rectTransform.localScale = Vector3.Lerp(originalScale * zoomFactor, originalScale, elapsedTime / zoomDuration);
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, originalRotation, elapsedTime / zoomDuration);
            newCanvas.anchoredPosition = Vector3.Lerp(new Vector3(-100, -500, 0), new Vector3(-100, -315, 0), elapsedTime / zoomDuration);
            elapsedTime = elapsedTime + Time.deltaTime;
            yield return null;
        }

        /*transform.position = originalPosition;
        transform.localScale = originalScale;
        transform.rotation = originalRotation;*/
        currentState = 0;
        //yield return null;        
    }
}
