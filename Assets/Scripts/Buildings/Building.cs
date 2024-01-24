using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class Building : MonoBehaviourPunCallbacks, ISelectable
{
    [SerializeField] private Transform exitPos;
    [SerializeField] private Transform entrancePos;
    [SerializeField] private float unloadTime = 1f;
    [SerializeField] private GameObject selectionCircle;
    [SerializeField] private List<UnitSO> unitsToSpawn;

    [field: SerializeField]
    public PlayerController player { get; private set; }

    private List<Gatherer> unitsInside = new List<Gatherer>();
    private List<float> timesOfArrival = new List<float>();

    [PunRPC]
    public void Initialize(Player player)
    {
        this.player = GameManager.Instance.GetPlayer(player.ActorNumber);
        this.player.buildings.Add(this);
    }

    void StoreUnit(Gatherer unit) {
        unitsInside.Add(unit);
        timesOfArrival.Add(Time.time);
        unit.gameObject.SetActive(false);
    }

    private void Update() {
        for (int i = 0; i < unitsInside.Count; i++) {
            if (Time.time - timesOfArrival[i] > unloadTime) {
                unitsInside[i].transform.position = exitPos.position;
                unitsInside[i].gameObject.SetActive(true);
                unitsInside[i].UnloadBackPack();
                unitsInside.RemoveAt(i);
                timesOfArrival.RemoveAt(i);
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && player.GetResource(ResourceType.Wood) >= 10 && player.buildings.Contains(this)) {
            player.AddResource(ResourceType.Wood, -10);
            //SpawnUnit();
        }
    }

    private void OnTriggerEnter(Collider other) {
        Gatherer unit = other.GetComponent<Gatherer>();

        if (unit != null) {
            StoreUnit(unit);
        }
    }

    public Vector3 GetEntrance() {
        return entrancePos.position;
    }

    public void Select() {
        selectionCircle.SetActive(true);
    }

    public void Deselect() {
        selectionCircle.SetActive(false);
    }
}
