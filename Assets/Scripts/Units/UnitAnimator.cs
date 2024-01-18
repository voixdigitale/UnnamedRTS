using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class UnitAnimator : MonoBehaviour
{
    public int IsGatheringHash = Animator.StringToHash("IsGathering");
    public int IsAttackingHash = Animator.StringToHash("IsAttacking");

    private Animator animator;
    private NavMeshAgent agent;
    private Unit unit;

    void OnEnable() {
        unit.OnStateChanged += HandleStateChanged;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();
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
            case UnitState.Moving:
            case UnitState.MovingToGather:
            case UnitState.MovingToBase:
            case UnitState.MovingToAttack:
                SetAllFalseExcept(-1);
                break;
            case UnitState.Attacking:
                SetAllFalseExcept(IsAttackingHash);
                break;
            case UnitState.Gathering:
                SetAllFalseExcept(IsGatheringHash);
                break;
        }
    }

    void SetAllFalseExcept(int boolHash) {
        animator.parameters
            .Where(x => x.type == AnimatorControllerParameterType.Bool)
            .ToList()
            .ForEach(x => animator.SetBool(x.nameHash, false));
        if (boolHash != -1)
            animator.SetBool(boolHash, true);
    }
}
