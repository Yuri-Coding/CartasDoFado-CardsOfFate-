using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
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

    //Vars para dar zoom
    [SerializeField] private float zoomFactor = 2.0f;
    private Vector3 zoomedPosition;
    private Vector3 zoomedScale;
    private float zoomDuration = 0.2f;
    private bool isZoomed = false;

    void Awake() 
    { 
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
        glowEffect.SetActive(false);
        playArrow.SetActive(false);

        zoomedScale = originalScale * zoomFactor;
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
                if (!Input.GetMouseButton(0))
                {
                    TransitionToState0();
                }
                break;
            case 4:
                if (!isZoomed)
                {
                    StartCoroutine(ZoomIn());
                }
                if (Input.GetMouseButtonDown(1))
                {
                    StartCoroutine(ZoomOut());
                }
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
        if(currentState == 1)
        {
            currentState = 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            originalPanelLocalPosition = rectTransform.localPosition;
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

        while (elapsedTime < zoomDuration)
        {
            rectTransform.localPosition = Vector3.Lerp(originalPosition, zoomedPosition, elapsedTime / zoomDuration);
            rectTransform.localScale = Vector3.Lerp(originalScale, zoomedScale, elapsedTime / zoomDuration);
            elapsedTime = elapsedTime + Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = zoomedPosition;
        rectTransform.localScale = zoomedScale;
    }

    private System.Collections.IEnumerator ZoomOut()
    {
        isZoomed = false;
        float elapsedTime = 0f;
        zoomedPosition = GetScreenCenterPosition();

        while(elapsedTime < zoomDuration)
        {
            rectTransform.localPosition = Vector3.Lerp(zoomedPosition, originalPosition, elapsedTime / zoomDuration);
            rectTransform.localScale = Vector3.Lerp(zoomedScale, originalScale, elapsedTime / zoomDuration);
            elapsedTime = elapsedTime + Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = originalPosition;
        rectTransform.localScale = originalScale;
    }
}
