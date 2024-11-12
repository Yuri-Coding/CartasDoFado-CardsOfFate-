using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System.Net;
using System.Net.Sockets;
using Unity.VisualScripting;

/*
 * botao e logica(parte) de exit, avisos de ip
 */

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button disconnectBtn;
    [SerializeField] private TMP_InputField HostIpField;
    [SerializeField] private TMP_Text avisoIp;

    public static NetworkManagerUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        disconnectBtn.onClick.AddListener(() =>
        {
            disconnectFunction();
        });

        SceneManager.sceneLoaded -= OnSceneLoad;
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {

        if (!string.Equals(scene.name, "GameMP")){
            return;
        }
        if (MainMenuUI.isHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = PlayerNetworkManager.Instance.ApprovalCheck;
            NetworkManager.Singleton.StartHost();
            GetLocalIPAddress();
            HostIpField.GameObject().SetActive(true);//Campo de ip deve aparecer quando vc é um host
            avisoIp.GameObject().SetActive(true);
        }
        else
        {
            setIp();
            NetworkManager.Singleton.StartClient();
            //GameObject field = HostIpField.GameObject();
            HostIpField.GameObject().SetActive(false);//Campo de ip não aparece quando vc é um client
            avisoIp.GameObject().SetActive(false);
        }

        Debug.Log("Nome do player: " + MainMenuUI.nameString);

    }

    private void setIp() // Set the IP from the input field
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = MainMenuUI.ipAddress;
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                HostIpField.text = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
    private void disconnectFunction()
    {
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        if (NetworkManager.Singleton.IsHost)
        {
            PlayerNetworkManager.Instance.ServerWipe();
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene(0);
        GameObjectCleanup();
        NetworkObjectCleanup();
        Debug.Log("disconnectFunction");
    }

    public void UpdateStatsField()
    {

    }

    public void GameObjectCleanup()
    {
        if (GameManagerMP.Instance != null)
        {
            Destroy(GameManagerMP.Instance.gameObject);
        }
    }

    public void NetworkObjectCleanup()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
}
