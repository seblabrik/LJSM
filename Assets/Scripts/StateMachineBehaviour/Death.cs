using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isDead", true);
        AnimationBool isDead = new AnimationBool { name = "isDead", value = true };
        animator.gameObject.transform.SendMessage("SaveAnimatorParameter", isDead);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.SendMessage("IsDead");
    }
}
