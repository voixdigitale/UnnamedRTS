using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;

public class Zombie : MonoBehaviourPunCallbacks, IDamageable {
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
        Dead
    }

    public State currentState;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float speed = 2f;
    public int attackDamage = 10;
    public int health = 50;

    public Transform currentTarget;

    public Animator animator;

    private bool canAttack = true;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Idle;
        currentTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Chasing:
                ChasingUpdate();
                break;
            case State.Attacking:
                AttackingUpdate();
                break;
        }
    }

    void ChasingUpdate()
    {
        if (currentTarget == null)
        {
            CheckForNearbyTargets();
            return;
        }

        EvaluateAttack();
    }

    void AttackingUpdate()
    {
        if (currentTarget == null)
        {
            CheckForNearbyTargets();
            return;
        }

        transform.LookAt(currentTarget);

        EvaluateAttack();

        if (canAttack)
        {
            Attack();
        }
    }

    public void SetTarget(Transform target)
    {
        if (currentState == State.Idle && target.gameObject.GetComponent<Unit>() != null && currentTarget == null)
        {
            currentTarget = target;

            EvaluateAttack();
        }
    }
    void SetState(State state)
    {
        switch (state)
        {
            case State.Idle:
                animator.SetBool("IsRunning", false);
                agent.isStopped = true;
                currentState = state;
                break;
            case State.Chasing:
                animator.SetBool("IsRunning", true);
                agent.isStopped = false;
                currentState = state;
                break;
            case State.Attacking:
                animator.SetBool("IsRunning", false);
                agent.isStopped = true;
                currentState = state;
                break;
        }
    }

    void Attack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        currentTarget.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, attackDamage);
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;

        if (currentTarget != null)
        {
            EvaluateAttack();
        }
        else
        {
            CheckForNearbyTargets();
        }
    }

    void CheckForNearbyTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 7);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.GetComponent<Unit>() != null)
            {
                currentTarget = collider.transform;
                return;
            }
        }

        SetState(State.Idle);
    }

    void EvaluateAttack()
    {
        agent.destination = currentTarget.position;

        if (Vector3.Distance(transform.position, agent.destination) > attackRange)
        {
            SetState(State.Chasing);
        } else {
            SetState(State.Attacking);
        }
    }

    [PunRPC]
    public void TakeDamage(int damage) {
        if (!photonView.IsMine) return;

        health -= damage;
        //OnDamageTaken?.Invoke(this, damage);

        if (health <= 0) {
            Die();
        }
    }

    void Die()
    {
        agent.isStopped = true;
        animator.SetTrigger("Death");
        currentState = State.Dead;
        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Destroy(gameObject);
    }
}
