using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


/*
 * seleciona id do alvo clickado, vai ser removido
 */

public class PlayerClickHandler : MonoBehaviour
{

    private void OnMouseDown()
    {
        // Verifica se o jogador atual é o dono do objeto
        if (TryGetComponent(out NetworkObject networkObject))
        {
            ulong playerId = networkObject.OwnerClientId;
            PlayerClicked(playerId);
        }
    }

    private void PlayerClicked(ulong targetId)
    {
        // Verifica se o jogador atual é o dono do objeto
        PlayerNetworkManager.Instance.GetLocalPlayerTarget(targetId);
        Debug.Log("Player with ID " + targetId + " was clicked!");
    }
}

