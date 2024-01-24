using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PlayerController : MonoBehaviourPun, IPunObservable {

    public static event Action OnResourceCollected;
    public static PlayerController me;
    public static PlayerController enemy;

    [HideInInspector]
    public int id;
    public Player photonPlayer;

    [Header("Units")]
    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();
    
    private SelectionManager selectionManager;
    private CommandManager commandManager;

    private int wood, scrap;

    public void Awake() {
        selectionManager = GetComponent<SelectionManager>();
        commandManager = GetComponent<CommandManager>();
    }

    [PunRPC]
    public void Initialize(Player player) {
        photonPlayer = player;

        if (player.IsLocal) {
            me = this;
            selectionManager.Initialize(me);
        } else {
            enemy = this;
            selectionManager.enabled = false;
            commandManager.enabled = false;
        }

        id = player.ActorNumber;

        GameManager.Instance.players[id - 1] = this;
    }


    public void Start()
    {
        if (!photonView.IsMine) return;

        PlayerUI.Instance.Initialize(me);
        GameObject buildingObj = PhotonNetwork.Instantiate(GameManager.Instance.baseBuildingPrefabPath, GameManager.Instance.spawnPoints[id - 1].position, Quaternion.identity);
        Building buildingScript = buildingObj.GetComponent<Building>();
        buildingScript.photonView.RPC("Initialize", photonPlayer, true);
        buildingScript.photonView.RPC("Initialize",  RpcTarget.Others, false);

        // Give the player 2 gatherers
        for (int i = 0; i < 2; i++) {
            Vector3 spawnPosition = new Vector3(GameManager.Instance.spawnPoints[id - 1].position.x + i * 2f, GameManager.Instance.spawnPoints[id - 1].position.y + 1, GameManager.Instance.spawnPoints[id - 1].position.z - 6f);
            GameObject gathererObj = PhotonNetwork.Instantiate(GameManager.Instance.GatherUnitPrefabPath, spawnPosition, Quaternion.identity);
            Gatherer gathererScript = gathererObj.GetComponent<Gatherer>();
            gathererScript.photonView.RPC("Initialize", photonPlayer, true);
            gathererScript.photonView.RPC("Initialize",  RpcTarget.Others, false);
        }
    }


    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                wood += amount;
                break;
            case ResourceType.Scrap:
                scrap += amount;
                break;
        }
        OnResourceCollected?.Invoke();
    }

    public int GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood:
                return wood;
            case ResourceType.Scrap:
                return scrap;
        }

        return 0;
    }

    public int GetUnitCount()
    {
        return units.Count;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Sending Data
        }
        else
        {
            //Receiving Data
        }
    }

    public List<T> GetSelectedUnits<T>() {
        return selectionManager.currentSelection.OfType<T>().ToList();
    }

    public int GetCurrentSelectedCount<T>()
    {
        return selectionManager.currentSelection.OfType<T>().Count();
    }

    public void SpawnSelectionMarker(Vector3 position, float duration = 1f, Color? color = null)
    {
        selectionManager.SpawnSelectionMarker(position, duration, color);
    }

    public PlayerController GetOtherPlayer(PlayerController player) {
        return player == me ? enemy : me;
    }
}
