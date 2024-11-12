using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

/*
 * variaveis de network e metodos de update
 */
public class PlayerVariables : NetworkBehaviour
{
    public static PlayerVariables Instance;//instance pra usar em outros scripts

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public struct PlayerStats : INetworkSerializable//struct, não se preocupe com o INetworkSerializable
    {
        public int morale;
        public int influence;
        public int corruption;
        public int poison;
        public int mute;
        public int imune;
        public int paralyzed;
        public ulong playerId;
        public int role;//0: innocent, 1: medic, 2: impostor, 3: corrupt
        public int currentVotes;
        public string playerName;

        public PlayerStats(int moralityValue, int influenceValue, int corruptionValue, int poisonValue, int muteValue, int imuneValue, int paralyzedValue, int roleValue, ulong playerValue, int currentVotesValue, string playerNameValue)
        {
            morale = moralityValue;
            influence = influenceValue;
            corruption = corruptionValue;
            poison = poisonValue;
            mute = muteValue;
            imune = imuneValue;
            paralyzed = paralyzedValue;
            playerId = playerValue;
            role = roleValue;//0: innocent, 1: medic, 2: impostor, 3: corrupt
            currentVotes = currentVotesValue;
            playerName = playerNameValue;

        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref morale);
            serializer.SerializeValue(ref influence);
            serializer.SerializeValue(ref corruption);
            serializer.SerializeValue(ref poison);
            serializer.SerializeValue(ref mute);
            serializer.SerializeValue(ref imune);
            serializer.SerializeValue(ref paralyzed);
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref role);
            serializer.SerializeValue(ref currentVotes);
            serializer.SerializeValue(ref playerName);
        }
    }

    //lista de players com as structs, SÓ É MANIPULADA NO HOST, VAZIA NOS CLIENTS, MANIPULAR APENAS ATRAVÉS SERVERRPCS POR FAVOR
    //não toque, apenas no host, SE DER PROBLEMA ME CHAME
    public List<PlayerStats> playersList = new List<PlayerStats>();

    //altera o valor de uma variavel no client especificado, mandar o indice do status e depois o valor que vc quer somar a ele
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerStatServerRpc(ulong playerId, int variable, int newStat, string playerName = null)
    {
        Debug.Log(playerId);
        // Find player by playerId
        var playerIndex = playersList.FindIndex(player => player.playerId == playerId);
        Debug.Log(playerIndex);
        if (playerIndex == -1)
        {
            Debug.LogWarning("Player not found");
            return;
        }

        var playerVariables = playersList[playerIndex];
        switch (variable)
        {
            case 0:
                playerVariables.morale += newStat;
                if (playerVariables.morale < 0)
                {
                    playerVariables.morale = 0;
                }
                break;
            case 1:
                playerVariables.influence += newStat;
                if (playerVariables.influence < 0)
                {
                    playerVariables.influence = 0;
                }
                break;
            case 2:
                playerVariables.corruption += newStat;
                if (playerVariables.corruption < 0)
                {
                    playerVariables.corruption = 0;
                }
                break;
            case 3:
                playerVariables.poison += newStat;
                if (playerVariables.poison < 0)
                {
                    playerVariables.poison = 0;
                }
                break;
            case 4:
                playerVariables.mute += newStat;
                if (playerVariables.mute < 0)
                {
                    playerVariables.mute = 0;
                }
                break;
            case 5:
                playerVariables.imune += newStat;
                if (playerVariables.imune < 0)
                {
                    playerVariables.imune = 0;
                }
                break;
            case 6:
                playerVariables.paralyzed += newStat;
                if (playerVariables.paralyzed < 0)
                {
                    playerVariables.paralyzed = 0;
                }
                break;
            case 7:
                playerVariables.currentVotes += newStat;
                if (playerVariables.currentVotes < 0)
                {
                    playerVariables.currentVotes = 0;
                }
                break;
            case 8:
                playerVariables.role = newStat;
                break;
            case 9:
                playerVariables.playerName = playerName;
                break;
        }

        // Update the list entry with the modified struct
        playersList[playerIndex] = playerVariables;
        SendDataSetup(playerId);
    }

    //recupera um status de um player especifico por id, usado em debug
    [ServerRpc(RequireOwnership = false)]
    public void GetPlayerStatServerRpc(ulong playerId, int statLine)
    {
        var playerIndex = playersList.FindIndex(player => player.playerId == playerId);
        if (playerIndex == -1)
        {
            Debug.LogWarning("Player not found");
            return;
        }

        int stat;   
        string playerName = playersList[playerIndex].playerName;
        switch (statLine)
        {
            case 0:
                stat = playersList[playerIndex].morale;
                break;
            case 1:
                stat = playersList[playerIndex].influence;

                break;
            case 2:
                stat = playersList[playerIndex].corruption;

                break;
            case 3:
                stat = playersList[playerIndex].poison;

                break;
            case 4:
                stat = playersList[playerIndex].mute;
                break;
            case 5:
                stat = playersList[playerIndex].imune;

                break;
            case 6:
                stat = playersList[playerIndex].paralyzed;
                break;
            case 7:
                stat = playersList[playerIndex].role;
                break;
            default:
                stat = 0;
                break;
        }
        Debug.Log("Player with id " + playerId + " and name " + playerName + " has statline " + statLine + " at value " + stat);
    }

    //pre-tratamento de dados antes de enviar para o alvo, NUNCA CHAMAR SendDataToClientClientRpc, SEMPRE CHAMAR ESSA, principalmente em serverRpcs
    public void SendDataSetup(ulong playerId)
    {
        Debug.Log("SendDataSetup player id: " + playerId);
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { playerId }
            }
        };
        var playerIndex = playersList.FindIndex(player => player.playerId == playerId);
        if (playerIndex == -1)
        {
            Debug.LogWarning("Player not found");
            return;
        }
        var playerVariables = playersList[playerIndex];
        SendDataToClientClientRpc(playerId, playerVariables, clientRpcParams);
    }

    //envia a struct do host para o client, o client atualiza sua struct local
    [ClientRpc]
    public void SendDataToClientClientRpc(ulong playerId, PlayerStats playerVariables, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("SendDataToClientClientRpc player id: " + playerId);
        PlayerNetworkManager.Instance.localStats = playerVariables;
        NetworkManagerUI.Instance.UpdateStatsField();
    }


    //remove o player da lista, caso for host, da wipe nela
    [ServerRpc(RequireOwnership = false)]
    public void AlterPlayerListOnDisconnectServerRpc(ulong playerId)
    {
        if (playerId == 0)
        {
            playersList.Clear();
        }
        else
        {
            var playerIndex = playersList.FindIndex(player => player.playerId == playerId);
            if (playerIndex == -1)
            {
                Debug.LogWarning("Player not found");
                return;
            }
            var playerVariables = playersList[playerIndex];
            playersList.Remove(playerVariables);
        }
    }

}
//COMO QUE FUNCIONA?
/*
 * Criação
 *  cria public List<PlayerStats> playersList = new List<PlayerStats>();
 *  existe em todos os clients, MAS SÓ OS HOTS VAI MEXER NELA, logo, esta vazia em outros clients
 *  quando um player é aprovado ApprovalCheck em PlayerNetworkManager(essa função só é executada no host), uma struct com status default é colocada na lista
 *  depois atualiza localStats em PlayerNetworkManager com os mesmos dados da nova struct
 *  
 * Atualização
 *  chama PlayerVariables.Instance.UpdatePlayerStatServerRpc, manda a info necessaria
 *  essa func é executada exclusivamente no server (serverRpc unity google pesquisar), vai atualizar a lista de players
 *  depois chama SendDataSetup, um pre-tratamento dos dados que vai mandar para o client
 *  manda os dados na SendDataToClientClientRpc, manda somente a tupla do client de interesse, apenas para o client de interesse(achei que não dava, oopps)
 *  
 * Exclusão
 *  HandleClientDisconnect é um listener que ta no PlayerNetworkManager, chama OnPlayerDisconnect
 *  depois, se achar o client que desconectou, chama AlterPlayerListOnDisconnectServerRpc
 *  AlterPlayerListOnDisconnectServerRpc acha o client e tira ele da lista
 *  se for host da wipe nela, host sempre tem id 0, A NÃO SER QUE UMA LOGICA DE HOST SWAP SEJA CRIADA, não foi o nosso caso, até o momento
 */