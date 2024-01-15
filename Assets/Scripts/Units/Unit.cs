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

public abstract class Unit : MonoBehaviour, ISelectable
{
    public event Action<UnitState> OnStateChanged;

    [field: SerializeField]
    public Player player { get; protected set;}
    public Building homeBase;

    [Header("Unit Setup")]
    [SerializeField] protected GameObject selectionCircle;
    [SerializeField] protected GameObject weapon;

    [Header("Unit Data")]
    [SerializeField] protected UnitSO unitData;

    protected UnitState state = UnitState.Idle;
    protected NavMeshAgent agent;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = unitData.MoveSpeed;
    }

    protected void SetState(UnitState newState)
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

    protected virtual void Update()
    {
        switch (state)
        {
            case UnitState.Moving:
                MovingUpdate();
                break;
        }
    }

    protected virtual void MovingUpdate()
    {
        if (Vector3.Distance(transform.position, agent.destination) == 0f)
        {
            agent.speed = unitData.MoveSpeed;
            SetState(UnitState.Idle);
        }
    }

    public virtual void Move(Vector3 destination)
    {
        agent.isStopped = false;
        agent.speed = Random.Range(agent.speed - 0.1f, agent.speed + 0.1f); //Add a bit of randomness to the speed
        agent.SetDestination(destination);

        SetState(UnitState.Moving);
    }
}
