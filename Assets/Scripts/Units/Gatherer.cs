using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Unit
{
    [Header("Gathering Backpack")]
    public ResourceType backpackResource = ResourceType.None;
    public int backpackQuantity = 0;
    [SerializeField] private GameObject backpack;

    private float lastGatherTime = 0f;

    private Resource currentResource;
    private Resource previousResource;
    private Vector3 previousDestination;

    protected override void Update() {
        base.Update();

        switch (state) {
            case UnitState.MovingToGather:
                MovingToGatherUpdate();
                break;
            case UnitState.Gathering:
                GatheringUpdate();
                break;
        }
    }

    void MovingToGatherUpdate() {
        if (currentResource == null) {
            SetState(UnitState.Idle);
            return;
        }

        if (Vector3.Distance(transform.position, agent.destination) <= 0.05f) {
            weapon.SetActive(true);
            backpack.SetActive(false);
            SetState(UnitState.Gathering);
            lastGatherTime = Time.time;
        }
    }

    void GatheringUpdate() {
        if (currentResource == null) {
            weapon.SetActive(false);
            SetState(UnitState.Idle);
            return;
        }

        transform.LookAt(currentResource.transform);

        if (Time.time - lastGatherTime > unitData.GatherRate) {
            lastGatherTime = Time.time;
            currentResource.Collect(unitData.GatherAmount, this);
            previousDestination = agent.destination;
            previousResource = currentResource;
            agent.destination = homeBase.GetEntrance();
            weapon.SetActive(false);
            backpack.SetActive(true);
            SetState(UnitState.MovingToBase);
        }
    }

    public void Gather(Resource resource, Vector3 destination) {
        currentResource = resource;

        agent.isStopped = false;
        agent.SetDestination(destination);

        SetState(UnitState.MovingToGather);
    }

    public void UnloadBackPack() {
        player.AddResource(backpackResource, backpackQuantity);
        backpackResource = ResourceType.None;
        backpackQuantity = 0;
        backpack.SetActive(false);
        Gather(previousResource, previousDestination);
    }
}
