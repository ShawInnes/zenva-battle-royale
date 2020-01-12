using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPun
{
    public float postGameTime;

    [Header("Players")]
    public string playerPrefabLocation;

    public PlayerController[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;

    private int playersInGames;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("PlayerInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void PlayerInGame()
    {
        playersInGames++;

        if (PhotonNetwork.IsMasterClient && playersInGames == PhotonNetwork.PlayerList.Length)
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer()
    {
        // create new player at a random spawn point
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        // initialize the player for all other players
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)
    {
        return this.players.First(p => p != null && p.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        return this.players.First(p => p != null && p.gameObject == playerObject);
    }

    public void CheckWinCondition()
    {
        if (alivePlayers == 1)
            photonView.RPC("WinGame", RpcTarget.All, players.First(p => !p.dead).id);
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {
        GameUI.instance.SetWinText(players.First(p => p.id == winningPlayer).photonPlayer.NickName);

        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMention()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }
}
