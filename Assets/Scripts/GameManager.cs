using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Quaternion = UnityEngine.Quaternion;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [Header("Players")]
    public string playerPrefabPath;
    public string baseBuildingPrefabPath;

    public Transform[] spawnPoints;
    public PlayerController[] players;

    [Header("Components")]
    [SerializeField] public RectTransform selectionBox; //The selection box UI element, needed for player initialization

    private int playersInGame;

    void Awake() {
        Instance = this;
        // PhotonNetwork.OfflineMode = true; //For testing
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("PlayerIsReady", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void PlayerIsReady() {
        playersInGame++;
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
            
    }

    void SpawnPlayer() {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, Vector3.zero, Quaternion.identity);
        PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId) {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObj) {
        return players.First(x => x.gameObject == playerObj);
    }
}
