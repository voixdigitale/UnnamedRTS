using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using Photon.Pun;
using Photon.Realtime;
using FischlWorks_FogWar;
using static FischlWorks_FogWar.csFogWar;

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

public abstract class Unit : MonoBehaviourPun, ISelectable, IDamageable
{
    public static event Action<Unit, int> OnDamageTaken;

    public event Action<UnitState> OnStateChanged;

    [field: SerializeField]
    public PlayerController player { get; protected set;}
    public HomeBuilding homeBase;

    [Header("Unit Setup")]
    [SerializeField] protected GameObject selectionCircle;
    [SerializeField] protected GameObject weapon;

    [Header("Unit Data")]
    [SerializeField] protected UnitSO unitData;
    [SerializeField] protected ActionButtonSO[] unitActionUI;

    protected UnitState state = UnitState.Idle;
    protected NavMeshAgent agent;

    protected Transform target;
    protected float currentHealth;
    protected float maxHealth;
    protected float lastAttackTime;
    protected int fogRevealerID;


    [PunRPC]
    public void Initialize(bool isMine) {
        if (isMine) {
            player = PlayerController.me;
            homeBase = (HomeBuilding) PlayerController.me.buildings[0];
            fogRevealerID = GameManager.Instance.fogWar.AddFogRevealer(new csFogWar.FogRevealer(transform, unitData.SightRange, false));
        } else {
            player = PlayerController.enemy;
            homeBase = (HomeBuilding) PlayerController.enemy.buildings[0];
        }
        
        currentHealth = maxHealth = unitData.MaxHealth;

        player.units.Add(this);
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = unitData.MoveSpeed;
    }

    public void SetState(UnitState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(newState);

        if (newState == UnitState.Idle)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    public void SetAgentDestination(Vector3 destination)
    {
        if (agent == null) return;

        //Check if the agent has been placed in a navmesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Agent " + gameObject.name + " is not on a navmesh!");
            return;
        }

        agent.isStopped = false;
        agent.speed = unitData.MoveSpeed;
        agent.speed = Random.Range(agent.speed - 0.1f, agent.speed + 0.1f); //Add a bit of randomness to the speed
        agent.SetDestination(destination);
        SetState(UnitState.Moving);

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
            case UnitState.MovingToAttack:
                MovingToAttackUpdate();
                break;
            case UnitState.Attacking:
                AttackUpdate();
                break;
        }
    }

    protected virtual void MovingUpdate()
    {
        if (Vector3.Distance(transform.position, agent.destination) == 0f)
        {
            SetState(UnitState.Idle);
        }
    }

    protected virtual void MovingToAttackUpdate() {
        if (target == null) SetState(UnitState.Idle);

        agent.destination = target.position;

        if (Vector3.Distance(transform.position, agent.destination) <= unitData.AttackRange) {
            agent.isStopped = true;
            SetState(UnitState.Attacking);
            lastAttackTime = Time.time;
        }
    }

    protected virtual void AttackUpdate() {
        if (target == null)
        {
            SetState(UnitState.Idle);
            return;
        }
        
        if (lastAttackTime + unitData.AttackRate < Time.time && target.GetComponent<IDamageable>() != null)
        {
            transform.LookAt(target);
            target.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, unitData.AttackDamage);
            lastAttackTime = Time.time;
        }

        if (target != null && Vector3.Distance(transform.position, target.position) > unitData.AttackRange) {
            SetAgentDestination(target.position);
            SetState(UnitState.MovingToAttack);
        }
    }

    public virtual void Attack(Transform objective, Vector3 destination) {
        agent.isStopped = false;
        agent.speed = Random.Range(agent.speed - 0.1f, agent.speed + 0.1f); //Add a bit of randomness to the speed
        agent.SetDestination(destination);

        target = objective;
        SetState(UnitState.MovingToAttack);
    }

    public virtual void Stop() {
        agent.isStopped = true;
        agent.ResetPath();
        SetState(UnitState.Idle);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;

        currentHealth -= damage;
        OnDamageTaken?.Invoke(this, damage);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        GameManager.Instance.fogWar.RemoveFogRevealer(fogRevealerID);
        player.units.Remove(this);
        PhotonNetwork.Destroy(gameObject);
    }
    public ActionButtonSO[] ActionButtons => unitActionUI;
}
