using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers = 10;

    public static NetworkManager instance;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)this.maxPlayers;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // are we in the game scene?
        if (GameManager.instance)
        {
            // update alive players
            GameManager.instance.alivePlayers--;
            GameUI.instance.UpdatePlayerInfoText();

            // if we're the master client, check the win condition
            if (PhotonNetwork.IsMasterClient)
                GameManager.instance.CheckWinCondition();
        }
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
