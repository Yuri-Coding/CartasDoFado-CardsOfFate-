using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteHandler : MonoBehaviour
{
    public Popup popup;
    public PlayerManager playerManager;
    private List<Player> playerList;


    // Start is called before the first frame update
    void Start()
    {
        playerList = playerManager.GetAllPlayers();
    }

    public void castVote(int index)
    {
        playerList[index].votesReceived += 1;
        Debug.Log($"Voto feito em: {playerList[index].PlayerName}");
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Attack, gameObject.transform.localPosition);
        GameManager.Instance.alreadyVoted = true;
    }

    public void castBotVote(int index)
    {
        playerList[index].votesReceived += 1;
    }

    public void SkipVote()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Positioning, gameObject.transform.localPosition);
        GameManager.Instance.alreadyVoted = true;
    }
}