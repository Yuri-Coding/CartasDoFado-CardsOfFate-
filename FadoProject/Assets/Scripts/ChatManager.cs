using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.VisualScripting;

/*
 * chat + toggle chat
 */
public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;

    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private CanvasGroup chatContent;
    [SerializeField] private ScrollRect chatWindow;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Button chatToggle;
    
    public string playerName;

    void Awake() 
    { 
        ChatManager.Singleton = this;
        playerName = MainMenuUI.nameString;
        chatToggle.onClick.AddListener(()=>{
            toggleChat();
        });
    }

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(chatInput.text, playerName);
            chatInput.text = "";
        }
    }


    public void SendChatMessage(string _message, string _fromWho = null)
    { 
        if(string.IsNullOrWhiteSpace(_message)) return;

        string S = _fromWho + " -> " +  _message;
        if(PlayerNetworkManager.Instance.localStats.mute <= 0)
        {
            SendChatMessageServerRpc(S);
        }
    }

    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    void SendChatMessageServerRpc(string message)
    {
        ReceiveChatMessageClientRpc(message);
    }

    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message)
    {
        ChatManager.Singleton.AddMessage(message);
    }

    void toggleChat(){
        Debug.Log("toggleChat, chatContentGameObject", chatContent.GameObject());
        chatWindow.GameObject().SetActive(!chatWindow.GameObject().activeSelf);
        chatInput.GameObject().SetActive(!chatInput.GameObject().activeSelf);
    }
}
