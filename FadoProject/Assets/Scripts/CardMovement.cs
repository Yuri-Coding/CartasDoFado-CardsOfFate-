using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using FadoProject;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	private RectTransform rectTransform;

	private GameObject canvasObject;
	private RectTransform newCanvas;
	private GameObject newCanvasObject;//N tem nenhuma rela鈬o com canvas kkk, t・pegando o handPos s・msm mas deu pregui軋 de mudar o nome
	private Canvas canvas;
	private Vector2 originalLocalPointerPosition;
	private Vector3 originalPanelLocalPosition;
	private Vector3 originalScale;
	private int currentState = 0;
	private Quaternion originalRotation;
	private Vector3 originalPosition;

	// Importar informação do Card
	public Card currentCard;
	public CardDisplay currentCardDisplay;

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

	//Var para encontrar o popUp
	private Popup popup;

	//Var para encontrar a detecção do clique no Dummy
	private ClickDummyDetect clickDummy;


	void Awake() 
	{
		newCanvasObject = GameObject.Find("HandPos");
		newCanvas = newCanvasObject.GetComponent<RectTransform>();
		//newCanvas.anchoredPosition = new Vector3(1000,1000,1); Da uma testadinha aqui
		//Pegando o script "Popup.cs" dentro do Popup manager
		popup = GameObject.Find("PopupManager").GetComponent<Popup>();
		//VAI TER QUE REFATORAR, FUNCIONA SÓ PRO DUMMY E PRA APENAS 1 DUMMY
		clickDummy = GameObject.Find("Dummy").GetComponent<ClickDummyDetect>();

		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		originalScale = rectTransform.localScale;
		originalPosition = rectTransform.localPosition;
		originalRotation = rectTransform.localRotation;
		glowEffect.SetActive(false);
		playArrow.SetActive(false);

		currentCardDisplay = gameObject.GetComponent<CardDisplay>();
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

	//Volta a carta pro estado original dela (em rota鈬o, escala e posi鈬o)
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

			 ChangeCurrentState(1);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (currentState == 1)
		{
			TransitionToState0();
		}
	}

	// Função de Mudança de Estado
	// (o switch case será executado uma vez quando alterar o estado)
	void ChangeCurrentState(int newState)
	{
		currentState = newState;
		switch (currentState)
		{
			case 2:
				if (currentCard.cardType == 0)
				{
                    currentCardDisplay.closeTaskUI();
					popup.actionRemoveCard -= removeCard;
                }
				break;
			case 3:
				if (currentCard.cardType == 0)
				{
                    currentCardDisplay.updateTaskUI();
					popup.actionRemoveCard += removeCard;
                }
                break;
		}
	}

	//Destroi a carta de task depois que foi jogada
	private void removeCard()
	{
		popup.actionRemoveCard -= removeCard;
		HandManager handManager = FindAnyObjectByType<HandManager>();
        handManager.cardsInHand.Remove(gameObject);
        Destroy(gameObject);
        handManager.updateHandVisuals();
    }

	//Destroi as cartas com efeito de target depois de ser jogada
	private void removeTargetCard()
	{
		clickDummy.usedCardAction -= removeTargetCard;
        HandManager handManager = FindAnyObjectByType<HandManager>();
        handManager.cardsInHand.Remove(gameObject);
        Destroy(gameObject);
        handManager.updateHandVisuals();
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
					ChangeCurrentState(3);
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
		//Debug.Log(currentCard.cardName);
        rectTransform.localPosition = playPosition;
		rectTransform.localRotation = Quaternion.identity;
		if (currentCard.canTarget)
		{
			//Verificando se a carta pode targetar um alvo e destruindo ela após seu uso
            GameManager.Instance.inPlay = true;
			clickDummy.usedCardAction += removeTargetCard;
        }
		

		

		if(Input.mousePosition.y < cardPlay.y)
		{
            GameManager.Instance.inPlay = false;
			clickDummy.usedCardAction -= removeTargetCard;
            ChangeCurrentState(2);
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
