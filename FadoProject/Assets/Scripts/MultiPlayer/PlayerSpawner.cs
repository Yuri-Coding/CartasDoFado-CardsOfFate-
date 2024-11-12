//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Netcode;

///*
// * parte da logica de spawn, vai ser removido
// */
//public class PlayerSpawner : NetworkBehaviour
//{
//    private SpriteRenderer spriteRenderer;

//    // NetworkVariable to store the sprite index and sync it across clients
//    private NetworkVariable<int> spriteIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

//    private void Awake()
//    {
//        spriteRenderer = GetComponent<SpriteRenderer>();
//    }

//    public override void OnNetworkSpawn()
//    {
//        if (IsClient)
//        {
//            // Hook into the sprite index change event to update the sprite when it changes
//            spriteIndex.OnValueChanged += OnSpriteIndexChanged;
//        }

//        // If this is the owner (local player), set the sprite on the server
//        if (IsOwner && IsHost)
//        {
//            // Only set the sprite for the owner (the one who spawned)
//            SetSprite();
//        }

//        // Apply the sprite immediately if it's already set
//        OnSpriteIndexChanged(0, spriteIndex.Value);
//    }

//    public override void OnDestroy()
//    {
//        if (spriteIndex != null)
//        {
//            spriteIndex.OnValueChanged -= OnSpriteIndexChanged;
//        }
//    }

//    // Method to update the sprite when the index changes
//    private void OnSpriteIndexChanged(int oldIndex, int newIndex)
//    {
//        Sprite newSprite = PlayerNetworkManager.Instance.GetSpriteByIndex(newIndex);
//        spriteRenderer.sprite = newSprite;
//        Debug.Log("SpriteIndexChange");
//    }

//    // Call this from the server to set the sprite index and sync it with clients
//    [ServerRpc(RequireOwnership = false)]
//    public void SetSpriteServerRpc(int index)
//    {
//        spriteIndex.Value = index;
//        Debug.Log("ServerRpc set sprite index");
//    }

//    private void SetSprite()
//    {
//        // Assuming you get the sprite index from somewhere (e.g., PlayerManagerMP)
//        int randomSpriteIndex = PlayerNetworkManager.Instance.GetRandomSpriteIndex();
//        SetSpriteServerRpc(randomSpriteIndex);
//        Debug.Log("PlayerSpawner SetSprite");

//    }

//}
