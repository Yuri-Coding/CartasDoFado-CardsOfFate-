using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkVariables : NetworkBehaviour
{
    //Exemplo de implementação simples
    /* private NetworkVariable<int> testInt = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

     public override void OnNetworkSpawn()
     {
         testInt.OnValueChanged += (int previousValue, int newValue) =>
         {
             Debug.Log(OwnerClientId + "; Number: " + testInt.Value);
         };
     }

     private void Update()
     {
         if (!IsOwner)
         {
             return;
         }

         if (Input.GetKeyDown(KeyCode.R))
         {
             testInt.Value = Random.Range(0, 11);
         }
     }*/

    /*private NetworkVariable<PlayerStatus> playerStats = new NetworkVariable<PlayerStatus>(new PlayerStatus
    {
        role = 0,//0 innocent, 1 medic, 2 impostor, 3 corrupt
        life = 8,
        morality = 0,
        influence = 0,
        corruption = 0,
        isMute = false,
        isImune = false,
        isPoisoned = false,
        isStunned = false
    }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);*/

    public struct PlayerStatsStruct : INetworkSerializable
    {
        public int role;//0: innocent, 1: medic, 2: impostor, 3: corrupt
        public int life;
        public int morality;
        public int influence;
        public int corruption;
        public int poison;
        public bool isMute;
        public bool isImune;
        public bool isStunned;
        //public ulong playerId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref role);
            serializer.SerializeValue(ref life);
            serializer.SerializeValue(ref morality);
            serializer.SerializeValue(ref influence);
            serializer.SerializeValue(ref corruption);
            serializer.SerializeValue(ref poison);
            serializer.SerializeValue(ref isMute);
            serializer.SerializeValue(ref isImune);
            serializer.SerializeValue(ref isStunned);
            //serializer.SerializeValue(ref playerId);
        }
    }

    public NetworkVariable<PlayerStatsStruct> playerStats = new NetworkVariable<PlayerStatsStruct>(new PlayerStatsStruct
    {
        role = 0, //0: innocent, 1: medic, 2: impostor, 3: corrupt, 4: spectator/dead
        life = 8,
        morality = 0,
        influence = 0,
        corruption = 0,
        poison = 0,
        isMute = false,
        isImune = false,
        isStunned = false
    }, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        // Registrar mudanças nos stats
        playerStats.OnValueChanged += playerVariablesChanged;

    }

    private void playerVariablesChanged(PlayerStatsStruct oldStats, PlayerStatsStruct newStats)
    {
        Debug.Log("Player stats updated: " + newStats.role + ", " + newStats.life + ", " + newStats.morality + ", " + newStats.influence + ", " + newStats.corruption + ", " + newStats.poison + ", " + newStats.isMute + ", " + newStats.isStunned);
        // Aqui você pode atualizar a interface ou aplicar outras mudanças, como exibir a vida/moralidade etc.
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdateLifeServerRpc(-1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UpdateMoralityServerRpc(1);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            UpdateInfluenceServerRpc(2);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            UpdateCorruptionServerRpc(3);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            UpdateMuteServerRpc(true);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            UpdateImuneServerRpc(true);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UpdateStunnedServerRpc(true);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UpdatePoisonServerRpc(1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateLifeServerRpc(int lifeValue)
    {
        var stats = playerStats.Value;
        stats.life += lifeValue;
        if(stats.life < 0)
        {
            stats.life = 0;
        }
        playerStats.Value = stats;
        Debug.Log("UpdateLifeServerRpc");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMoralityServerRpc(int moralityValue)
    {
        var stats = playerStats.Value;
        stats.morality += moralityValue;
        if(stats.morality < 0)
        {
            stats.morality = 0;
        }
        playerStats.Value = stats;
        Debug.Log("UpdateMoralityServerRpc");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateInfluenceServerRpc(int influenceValue)
    {
        var stats = playerStats.Value;
        stats.influence += influenceValue;
        if (stats.influence < 0)
        {
            stats.influence = 0;
        }
        playerStats.Value = stats;
        Debug.Log("UpdateInfluenceServerRpc");
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void UpdateCorruptionServerRpc(int corruptionValue)
    {
        var stats = playerStats.Value;
        stats.corruption += corruptionValue;
        if (stats.corruption < 0)
        {
            stats.corruption = 0;
        }
        playerStats.Value = stats;
        Debug.Log("UpdateCorruptionServerRpc");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateMuteServerRpc(bool mute)
    {
        var stats = playerStats.Value;
        stats.isMute = mute;
        playerStats.Value = stats;
        Debug.Log("UpdateMuteServerRpc");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateStunnedServerRpc(bool stunned)
    {
        var stats = playerStats.Value;
        stats.isStunned = stunned;
        playerStats.Value = stats;
        Debug.Log("UpdateStunnedServerRpc");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateImuneServerRpc(bool imune)
    {
        var stats = playerStats.Value;
        stats.isImune = imune;
        playerStats.Value = stats;
        Debug.Log("UpdateImuneServerRpc");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePoisonServerRpc(int poison)
    {
        var stats = playerStats.Value;
        stats.poison += poison;
        if(stats.poison < 0)
        {
            stats.poison = 0;
        }
        playerStats.Value = stats;
        Debug.Log("UpdatePoisonServerRpc");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateRoleServerRpc(int roleValue)
    {
        var stats = playerStats.Value;
        stats.role = roleValue;
        playerStats.Value = stats;
        Debug.Log("UpdateRoleServerRpc");
    }

    //[ServerRpc(RequireOwnership = false)]
    //public void UpdatePlayerIdServerRpc(ulong playerId)
    //{
    //    var stats = playerStats.Value;
    //    stats.playerId = playerId;
    //    playerStats.Value = stats;
    //    Debug.Log("UpdatePlayerIdServerRpc");
    //}
}
