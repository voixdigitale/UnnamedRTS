using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PlayerController : MonoBehaviourPun, IPunObservable {

    public static event Action OnResourceCollected;

    [HideInInspector]
    public int id;

    [Header("Units")]
    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();

    [Header("Components")]
    public Player photonPlayer;
    
    private SelectionManager selectionManager;
    private CommandManager commandManager;

    private int wood, scrap;

    public void Awake() {
        selectionManager = GetComponent<SelectionManager>();
        commandManager = GetComponent<CommandManager>();
    }

    [PunRPC]
    public void Initialize(Player player) {
        if (!photonView.IsMine) return;

        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.Instance.players[id - 1] = this;
        
        selectionManager.Initialize(player);
    }


    public void Start()
    {

        /*GameObject buildingObj = PhotonNetwork.Instantiate(GameManager.Instance.baseBuildingPrefabPath, GameManager.Instance.spawnPoints[id - 1].position, Quaternion.identity);
        Building buildingScript = buildingObj.GetComponent<Building>();
        buildingScript.photonView.RPC("Initialize", RpcTarget.All, photonPlayer);*/
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
}
