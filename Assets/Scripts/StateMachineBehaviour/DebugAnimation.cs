using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAnimation : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("FightIdleController"))
        {
            Debug.Log("entersFightIdleController");
        }
        else if (stateInfo.IsName("IdleController"))
        {
            Debug.Log("entersIdleController");
        }
        else if (stateInfo.IsName("Main"))
        {
            Debug.Log("entersMain");
            Debug.Log("is right = " + animator.GetBool("right"));
        }
        else if (stateInfo.IsName("IdleLeft"))
        {
            Debug.Log("entersIdleLeft");
        }
        else if (stateInfo.IsName("IdleRight"))
        {
            Debug.Log("entersIdleRight");
        }
        else if (stateInfo.IsName("LocomotionController"))
        {
            Debug.Log("entersLocomotionController");
        }
        else if (stateInfo.IsName("WalkLeft"))
        {
            Debug.Log("entersWalkLeft");
        }
        else if (stateInfo.IsName("Right"))
        {
            Debug.Log("entersRight");
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("FightIdleController"))
        {
            Debug.Log("leavesFightIdleController");
        }
        else if (stateInfo.IsName("IdleController"))
        {
            Debug.Log("leavesIdleController");
        }
        else if (stateInfo.IsName("Main"))
        {
            Debug.Log("leavesMain");
        }
        else if (stateInfo.IsName("IdleLeft"))
        {
            Debug.Log("leavesIdleLeft");
        }
        else if (stateInfo.IsName("IdleRight"))
        {
            Debug.Log("leavesIdleRight");
        }
        else if (stateInfo.IsName("LocomotionController"))
        {
            Debug.Log("leavesLocomotionController");
        }
        else if (stateInfo.IsName("WalkLeft"))
        {
            Debug.Log("leavesWalkLeft");
        }
        else if (stateInfo.IsName("WalkRight"))
        {
            Debug.Log("leavesWalkRight");
        }
        else if (stateInfo.IsName("Right"))
        {
            Debug.Log("leavesRight");
        }

        if (animator.GetBool("right"))
        {
            Debug.Log("right");
        }
        if (animator.GetBool("left"))
        {
            Debug.Log("left");
        }
    }
}
