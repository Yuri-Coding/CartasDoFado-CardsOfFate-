using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteHandlerMP : MonoBehaviour
{
    public PopupMP popup;
    public PlayerManagerMP playerManager;
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
        GameManagerMP.Instance.alreadyVoted = true;
    }

    public void SkipVote()
    {
        GameManagerMP.Instance.alreadyVoted = true;
    }
}
