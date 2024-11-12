using FadoProject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandlerMP: MonoBehaviour
{
	// Adicionar Handlers
	public EffectHandlerMP effectHandler;
	public PlayerManagerMP playerManager;
	
	private List<Player> allTarget;			// Todos os targets selecionados
    private List<Player> eligibleTarget;    // Poss?veis targets

	private Card currentCard;

    public static TargetHandlerMP Instance { get; private set; }
	private int targetAmount;
	private bool isRandom;


    public event Action usedCardAction;		// Action para destruir a carta

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

	public void SetTargetConfig(Card card)
	{
        ResetTargeting();

        currentCard = card;
		Debug.Log($"SetTargetConfig iniciado para carta {currentCard.cardName}");
        
		int amount = 0;
		isRandom = true;
		switch(card.targetType)
		{
            case TargetType.Single:
				amount = 1;
				isRandom = false;
				break;

            case TargetType.Double:
				amount = 2;
				isRandom = false;
                break;

            case TargetType.Random:
				amount = 1;
				isRandom = true;
                break;

            // Obt?m Metade dos Eleg?veis
            case TargetType.Half:
				amount = eligibleTarget.Count / 2;
				isRandom = true;
                break;

            // Considera todos os eleg?veis como alvo
            case TargetType.All:
				allTarget = eligibleTarget;
				ApplyAll();
				return;
        }
		if (isRandom) {
			RandomTarget(amount);
			return;
		}
        targetAmount = amount;
		Debug.Log(targetAmount);
    }

	private void ResetTargeting()
	{
		targetAmount = 0;
		allTarget = new List<Player>();
		currentCard = new Card();
		eligibleTarget = playerManager.eligibleTarget;
	}

	public void RandomTarget(int amount)
	{
		while(amount > 0) {
			// Obt?m um player aleat?rio
			Player rtarget = eligibleTarget[UnityEngine.Random.Range(0, eligibleTarget.Count)];

			// Se o player j? n?o est? no allTarget, adicionar no allTarget
			if (allTarget.Contains(rtarget))
			{
				allTarget.Add(rtarget);
				amount--;
			}
        }
		
	}

	// Seleciona o target at? alcan?ar o n?mero de targets.
	public void SelectTarget(int playerId)
	{
		Debug.Log("Target Selecionado");
		if (targetAmount <= 0) return;

		// L?gica de Target
		allTarget.Add(playerManager.GetPlayerById(playerId));
		targetAmount--;




		// Se o player selecionou todos os targets
		if (targetAmount == 0)
		{

			ApplyAll();
			// =-=-=-= precisa mandar um sinal de q player realizou a a??o.
		}
	}
    private void ApplyAll()
    {
		Debug.Log("DEU APPLY!!!");
		if (currentCard.selfEffects.Count != 0) effectHandler.ApplySelf(GameManagerMP.Instance.mainPlayer, currentCard.selfEffects);
        effectHandler.ApplyAllTargetted(allTarget, currentCard.targetEffects);
		Debug.Log($"Os efeitos de {currentCard.cardName} foram aplicados.");

		GameManagerMP.Instance.mainPlayer.PerformAction();

        usedCardAction?.Invoke();
    }
}