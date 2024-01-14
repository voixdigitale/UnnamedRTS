using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Unit unit;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();

        unit.OnStateChanged += HandleStateChanged;
    }

    void OnDisable() {
        unit.OnStateChanged -= HandleStateChanged;
    }

    void Update()
    {
        animator.SetFloat("MoveSpeed", agent.velocity.magnitude); 
    }

    void HandleStateChanged(UnitState state)
    {
        switch (state)
        {
            case UnitState.Idle:
                animator.SetBool("IsGathering", false);
                break;
            case UnitState.Moving:
                animator.SetBool("IsGathering", false);
                break;
            case UnitState.MovingToGather:
                animator.SetBool("IsGathering", false);
                break;
            case UnitState.MovingToAttack:
                animator.SetBool("IsGathering", false);
                break;
            case UnitState.Attacking:
                animator.SetBool("IsGathering", false);
                break;
            case UnitState.Gathering:
                animator.SetBool("IsGathering", true);
                break;
        }
    }
}
