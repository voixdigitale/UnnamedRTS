using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieIdleBehaviour : StateMachineBehaviour
{
    [SerializeField] private float _idleSwitchTime = 3f;

    private float _timer = 0f;
    private int currentAnimation = 1;
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _timer += Time.deltaTime;

        animator.SetFloat("IdleAnimator", Mathf.Lerp(animator.GetFloat("IdleAnimator"), currentAnimation, Time.deltaTime));

        if (_timer >= _idleSwitchTime)
        {
            currentAnimation = Random.Range(0, 2);
            _timer = 0f;
        }
    }
}
