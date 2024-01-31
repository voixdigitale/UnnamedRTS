using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public abstract class Building : MonoBehaviourPunCallbacks, ISelectable
{
    
    [SerializeField] private GameObject selectionCircle;
    [SerializeField] private BuildingSO buildingData;
    [SerializeField] private ActionButtonSO[] buildingActionUI;

    [Header("UI")]
    [SerializeField] private GameObject buildingBar;
    [SerializeField] private Slider buildingProgress;

    [field: SerializeField]
    public PlayerController player { get; private set; }

    private bool isBusy = false;

    [PunRPC]
    public void Initialize(bool isMine)
    {
        if (isMine) {
            player = PlayerController.me;
        } else {
            player = PlayerController.enemy;
        }
        player.buildings.Add(this);
        GameManager.Instance.RefreshNavMesh();
    }

    public void Select() {
        selectionCircle.SetActive(true);
    }

    public void Deselect() {
        selectionCircle.SetActive(false);
    }

    public void TrySpawningUnit(UnitSO unit)
    {
        if (player.HasEnoughResources(unit.UnitCost) && !isBusy)
        {
            foreach (ProductionCost unitCost in unit.UnitCost)
            {
                player.RemoveResource(unitCost.Resource, unitCost.Amount);
            }

            isBusy = true;
            StartCoroutine(ProduceUnit(unit, unit.TimeToProduce));
        } else if (isBusy)
        {
            PlayerUI.Instance.ShowErrorMessage("Building is busy!");
        }

        else
        {
            PlayerUI.Instance.ShowErrorMessage("Not enough resources!");
        }
    }

    private IEnumerator ProduceUnit(UnitSO unit, float timeToProduce)
    {
        float timePassed = 0f;
        buildingBar.SetActive(true);
        buildingProgress.maxValue = timeToProduce;
        buildingProgress.value = 0f;

        while (timePassed < timeToProduce)
        {
            timePassed += Time.deltaTime;
            buildingProgress.value = timePassed;
            yield return null;
        }

        SpawnUnit(unit);
        buildingBar.SetActive(false);
        isBusy = false;
    }

    private void SpawnUnit(UnitSO unit)
    {
        Vector3 spawnPosition =
            new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - 6f);
        GameObject unitObj = PhotonNetwork.Instantiate(unit.UnitName, spawnPosition, Quaternion.identity);
        Unit unitScript = unitObj.GetComponent<Unit>();
        unitScript.photonView.RPC("Initialize", player.photonPlayer, true);
        unitScript.photonView.RPC("Initialize", RpcTarget.Others, false);
    }

    public ActionButtonSO[] ActionButtons => buildingActionUI;
}
