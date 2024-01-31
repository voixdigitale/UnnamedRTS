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

    [Header("Units")]
    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();

    [Header("Components")]
    public Player photonPlayer;
    [SerializeField] public CameraController cameraControl;

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
            cameraControl = GameManager.Instance.cameraControl;
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
        PlaceBuilding(GameManager.Instance.baseBuildingPrefabPath, GameManager.Instance.spawnPoints[id - 1].position);
        cameraControl.MoveCameraTo(GameManager.Instance.spawnPoints[id - 1].position);

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

    public bool RemoveResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood:
                if (wood < amount)
                    return false;
                wood -= amount;
                break;
            case ResourceType.Scrap:
                if (scrap < amount)
                    return false;
                scrap -= amount;
                break;
        }
        return true;
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

    public bool HasEnoughResources(List<ProductionCost> cost)
    {
        foreach (ProductionCost unitCost in cost)
        {
            if (GetResource(unitCost.Resource) < unitCost.Amount)
                return false;
        }

        return true;
    }

    public void TryPlaceBuilding(BuildingSO building, Vector3 position) {
        if (HasEnoughResources(building.BuildingCost))
        {
            foreach (ProductionCost cost in building.BuildingCost)
            {
                RemoveResource(cost.Resource, cost.Amount);
            }

            PlaceBuilding(building.BuildingPath, position);
        } else
        {
            PlayerUI.Instance.ShowErrorMessage("Not enough resources!");
        }

    }

    public void PlaceBuilding(string prefabPath, Vector3 position)
    {
        GameObject buildingObj = PhotonNetwork.Instantiate(prefabPath, position, Quaternion.identity);
        Building buildingScript = buildingObj.GetComponent<Building>();
        buildingScript.photonView.RPC("Initialize", photonPlayer, true);
        buildingScript.photonView.RPC("Initialize", RpcTarget.Others, false);
    }
}
