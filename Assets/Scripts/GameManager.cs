using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using FischlWorks_FogWar;
using Unity.AI.Navigation;
using Quaternion = UnityEngine.Quaternion;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [Header("Players")]
    public string playerPrefabPath;
    public string baseBuildingPrefabPath;
    public string GatherUnitPrefabPath;

    public Transform[] spawnPoints;
    public PlayerController[] players;

    [Header("Components")]
    [SerializeField] public RectTransform selectionBox; //The selection box UI element, needed for player initialization
    [SerializeField] public CameraController cameraControl;
    [SerializeField] public NavMeshSurface navMeshSurface;
    [SerializeField] public csFogWar fogWar;

    private int playersInGame;

    void Awake() {
        Instance = this;
        //PhotonNetwork.OfflineMode = true; //For testing
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

    public void RefreshNavMesh()
    {
        navMeshSurface.BuildNavMesh();
    }
}
