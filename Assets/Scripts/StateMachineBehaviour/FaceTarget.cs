using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 position = animator.gameObject.transform.position;

        bool left = (target.x < position.x);
        bool right = !left;

        AnimationBool boolRight = new AnimationBool { name = "right", value = right };
        AnimationBool boolLeft = new AnimationBool { name = "left", value = left };

        animator.SetBool("right", right);
        animator.gameObject.transform.SendMessage("SaveAnimatorParameter", boolRight);
        animator.SetBool("left", left);
        animator.gameObject.transform.SendMessage("SaveAnimatorParameter", boolLeft);

        animator.gameObject.transform.SendMessage("UpdateLateralOrientation");
    }
}