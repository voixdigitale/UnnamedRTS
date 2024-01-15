using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public enum UnitState
{
    Idle,
    Moving,
    MovingToGather,
    MovingToAttack,
    MovingToBase,
    Attacking,
    Gathering
}


public class Unit : MonoBehaviour, ISelectable
{
    public event Action<UnitState> OnStateChanged;

    [SerializeField] private GameObject selectionCircle;
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject backpack;

    [Header("Gatherer")]
    public float moveSpeed = 3.5f;
    public float gatherAmount = 10f;
    public float gatherRate = 1f;
    public float lastGatherTime = 0f;
    public int unitCost = 10;
    public ResourceType backpackResource = ResourceType.None;
    public int backpackQuantity = 0;
    public Building homeBase;
    public ResourceType unitCostResource = ResourceType.Wood;

    [field: SerializeField]
    public Player player { get; private set;}

    public UnitState state = UnitState.Idle;

    private NavMeshAgent agent;
    private Resource currentResource;
    private Resource previousResource;
    private Vector3 previousDestination;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void SetState(UnitState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(newState);

        if (newState == UnitState.Idle)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void Select()
    {
        selectionCircle.SetActive(true);
    }

    public void Deselect() {
        selectionCircle.SetActive(false);
    }

    void Update()
    {
        switch (state)
        {
            case UnitState.Moving:
                MovingUpdate();
                break;
            case UnitState.MovingToGather:
                MovingToGatherUpdate();
                break;
            case UnitState.Gathering:
                GatheringUpdate();
                break;
        }
    }
    
    void MovingUpdate()
    {
        if (Vector3.Distance(transform.position, agent.destination) == 0f)
        {
            agent.speed = moveSpeed;
            SetState(UnitState.Idle);
        }
    }


    void MovingToGatherUpdate()
    {
        if (currentResource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if (Vector3.Distance(transform.position, agent.destination) == 0f) {
            weapon.SetActive(true);
            backpack.SetActive(false);
            SetState(UnitState.Gathering);
            lastGatherTime = Time.time;
        }
    }

    void GatheringUpdate()
    {
        if (currentResource == null)
        {
            weapon.SetActive(false);
            SetState(UnitState.Idle);
            return;
        }

        transform.LookAt(currentResource.transform);

        if (Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time;
            currentResource.Collect(gatherAmount, this);
            previousDestination = agent.destination;
            previousResource = currentResource;
            agent.destination = homeBase.GetEntrance();
            weapon.SetActive(false);
            backpack.SetActive(true);
            SetState(UnitState.MovingToBase);
        }
    }

    public void Move(Vector3 destination)
    {
        agent.isStopped = false;
        agent.speed = Random.Range(agent.speed - 0.1f, agent.speed + 0.1f); //Add a bit of randomness to the speed
        agent.SetDestination(destination);

        SetState(UnitState.Moving);
    }

    public void Gather(Resource resource, Vector3 destination)
    {
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
