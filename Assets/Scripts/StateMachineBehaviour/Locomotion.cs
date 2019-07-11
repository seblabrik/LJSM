﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.SendMessage("UpdateDirection");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("isMoving")) { animator.gameObject.transform.SendMessage("UpdateDirection"); }
    }
}
