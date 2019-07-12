﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralOrientation : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.SendMessage("UpdateLateralOrientation");
    }
}