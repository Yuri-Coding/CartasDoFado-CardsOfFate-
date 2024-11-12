using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System;

/*
 * Logica de spawn e desconect, vai sofrer mudanças drasticas
 */

public class PlayerNetworkManager : MonoBehaviour
{
    //[SerializeField] private Sprite[] playerSprites; // Array to hold the 10 possible sprites

    //private List<Vector2> availablePositions = new List<Vector2>(); // List to hold available positions
    //private List<Sprite> availableSprites = new List<Sprite>(); // List to hold available sprites
    //private Dictionary<ulong, PlayerData> playerData = new Dictionary<ulong, PlayerData>(); // Store player data

    public static PlayerNetworkManager Instance;

    public PlayerVariables.PlayerStats localStats;//status locais atualizadas a partir da playersList

    public ulong currentTarget = 99;//default

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

        //InitializeAvailablePositions();
        //InitializeAvailableSprites();
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(currentTarget);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerVariables.Instance.UpdatePlayerStatServerRpc(currentTarget, 0, 1);//forma padrão de dar update em uma varivel
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PlayerVariables.Instance.UpdatePlayerStatServerRpc(currentTarget, 0, -1);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerVariables.Instance.UpdatePlayerStatServerRpc(NetworkManager.Singleton.LocalClientId,5,1);
        }
    }

    public void GetLocalPlayerTarget(ulong targetId)
    {
        currentTarget = targetId;
        Debug.Log("Updated current target to player with ID: " + targetId);
    }

    //LOGICA DE SPAWN
    //private void InitializeAvailablePositions()
    //{
    //    availablePositions = new List<Vector2>
    //    {
    //        new Vector2(-1.5f, 1.5f),
    //        new Vector2(1.5f, 1.5f),
    //        new Vector2(-3.5f, 2.0f),
    //        new Vector2(3.5f, 1.5f),
    //        new Vector2(-5.0f, 2.5f),
    //        new Vector2(5.0f, 2.5f),
    //        new Vector2(-4.4f, 3.0f),
    //        new Vector2(4.4f, 3.0f),
    //        new Vector2(-6.2f, 4.2f),
    //        new Vector2(6.2f, 4.2f)
    //    };
    //}

    //LOGICA DE SPAWN
    //private void InitializeAvailableSprites()
    //{
    //    availableSprites = new List<Sprite>(playerSprites);
    //}

    //LOGICA DE DISCONECT
    //RETORNA O SPRITE E POSIÇÃO
    public void OnPlayerDisconnect(ulong clientId)
    {
        Debug.Log("OnplayerDisconnect");
        //if (playerData.ContainsKey(clientId))
        //{
            //availablePositions.Add(playerData[clientId].Position);
            //availableSprites.Add(playerData[clientId].Sprite);
            //playerData.Remove(clientId);
            PlayerVariables.Instance.AlterPlayerListOnDisconnectServerRpc(clientId);
            MainMenuUI.nameString = null;
            //Debug.Log("OnplayerDisconnect players sobrando: " + playerData.Count);
        //}
    }

    private void HandleClientDisconnect(ulong clientId)//LISTENER
    {
        OnPlayerDisconnect(clientId);
        Debug.Log("Player disconnected: " + clientId);
    }


    //LOGICA DE SPAWN
    //APROVAÇÃO ANTES DE CONECTAR
    public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Inicio ApprovalCheck");
        //if (playerData.Count >= 10)//!!!!!TODO: COLOCAR UMA MENSAGEM NA TELA DO PLAYER QUE É REJEITADO
        //{
        //    response.Approved = false;
        //    response.Reason = "Testing the declined approval message";
        //    Debug.Log("Lobby cheio");
        //    return;
        //}
        var clientId = request.ClientNetworkId;
        Debug.Log("ApprovalCheck id: " + clientId);
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;

        // Assign a random position and sprite to the player
        //Vector2 randomPosition = GetRandomPosition();
        //Sprite randomSprite = GetRandomSprite();

        // Store player data for later (cleanup on disconnect)
        //playerData.Add(clientId, new PlayerData(randomPosition, randomSprite));

        // Set the player's spawn position
        //response.Position = randomPosition;

        response.Position = new Vector2(0,0);

        // Set sprite after player object spawns
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (id == clientId)
            {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
                if (playerObject != null)
                {
                    // var playerController = playerObject.GetComponent<PlayerSpawner>();
                     //Debug.Log(playerController);
                    //if (playerController != null)
                    //{
                    //    //int spriteIndex = Array.IndexOf(playerSprites, randomSprite);
                    //    //playerController.SetSpriteServerRpc(spriteIndex);
                    //}

                    PlayerVariables.PlayerStats newPlayer = new(0, 0, 0, 0, 0, 0, 0, 0, clientId, 0, MainMenuUI.nameString);//cria nova struct pra ser adicionada na playersList
                    PlayerVariables.Instance.playersList.Add(newPlayer);//adiciona struct nova a playersList
                    localStats = newPlayer;//Pega os stats da nova struct na lista local
                    NetworkManagerUI.Instance.UpdateStatsField();//atualiza os campos visuais
                }
            }
        };
        //Debug.Log("Players: " + playerData.Count);
        //Debug.Log("Posições sobrando: " + availablePositions.Count);
        //Debug.Log("Sprites sobrando: " + availableSprites.Count);
    }

    //LOIGICA DE SPAWN
    //private Vector2 GetRandomPosition()
    //{
    //    if (availablePositions.Count > 0)
    //    {
    //        int index = UnityEngine.Random.Range(0, availablePositions.Count);
    //        Vector2 position = availablePositions[index];
    //        availablePositions.RemoveAt(index); // Remove the position from available list
    //        return position;
    //    }

    //    // Default to (0, 0) if no position is available (this shouldn't happen with 10 players max)
    //    return Vector2.zero;
    //}

    //LOGICA DE SPAWN
    //private Sprite GetRandomSprite()
    //{
    //    if (availableSprites.Count > 0)
    //    {
    //        int index = UnityEngine.Random.Range(0, availableSprites.Count);
    //        Sprite sprite = availableSprites[index];
    //        availableSprites.RemoveAt(index); // Remove the sprite from available list
    //        return sprite;
    //    }

    //    return null; // Return null if no sprite is available
    //}

    //LOGICA DE SPAWN
    //public int GetRandomSpriteIndex()
    //{
    //    if (availableSprites.Count > 0)
    //    {
    //        int index = UnityEngine.Random.Range(0, availableSprites.Count);
    //        availableSprites.RemoveAt(index); // Remove the sprite from available list
    //        return index;
    //    }

    //    return -1; // Return -1 if no sprite is available
    //}

    //LOGICA DE SPAWN
    //PEGA A POSIÇÃO DO SPRITE, USA NO PALYERSPAWNER QUE FICA NO PLAYER
    //public Sprite GetSpriteByIndex(int index)
    //{
    //    if (index >= 0 && index < playerSprites.Length)
    //    {
    //        return playerSprites[index];
    //    }
    //    return null; // Fallback in case of an invalid index
    //}


    //Caso o Host saia, chamar essa função, REINICIA O LOBBY
    public void ServerWipe()
    {
        //playerData = null;
        //playerData = new Dictionary<ulong, PlayerData>();
        //InitializeAvailablePositions();
        //InitializeAvailableSprites();
        PlayerVariables.Instance.AlterPlayerListOnDisconnectServerRpc(0);
        Debug.Log("ServerWipe");
    }

}


//ARMAZENA POS E SPRITE DO PLAYER
public class PlayerData
{
    public Vector2 Position { get; private set; }
    public Sprite Sprite { get; private set; }

    public PlayerData(Vector2 position, Sprite sprite)
    {
        Position = position;
        Sprite = sprite;
    }
}


/*
 * Conexão player
 *  MainMenu manda nome player e Ishost para NetworkManagerUI, que tem um listener de troca de cena
 *  listener verifica se é host ou client, faz mudança apropriadas
 *  Vai para approvalCheck em PlayerNetworkManager, só ocorre no host
 *  logica de spawn(vai mudar)
 *  instancia struct do player novo an lista de players no host
 *  pega a nova struct e armazena localmente
 *  
 * Disconnect
 *  OnPlayerDisconnect tem um listener
 *  tira o cara da lista e disconecta ele
 *  anula o nome também
 *  Caso for host, da Wipe na lista
 *  Também destroy o gameManager e NetworkManager, pra não fuder tudo
 *  Começa no NetworkManagerUI, vai pro listener, depois volta pro main menu
 */