using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeZombie : Enemy
{

    protected override void Start()
    {
        base.Start();
        rangeAttack = 5f;
        aggroRange = 5f;
    }

    protected override (bool, float) DoActionInTurn(bool isMoving, float movingTimer)
    {
        var target = player.transform.position;
        target.z = 0;

        //Fonction d'attaque
        bool canAttack = false;
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, target);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == "Player" && hit.distance <= rangeAttack)
            {
                canAttack = true;
                isMoving = false;
                agent.ResetPath();
                RangeAttack(target);
                break;
            }
        }
        if (!canAttack)
        {
            if (isMoving)
            {
                ap = Math.Max(0f, ap - (Time.time - movingTimer) * unitStat.apMovingCost);
                movingTimer = Time.time;
                if (HasReachedDestination()) { isMoving = false; }
            }
            else
            {
                //Fonction déplacement
                if ((new Vector3(target.x - 0.75f, target.y, 0f) - transform.position).sqrMagnitude < (new Vector3(target.x + 0.75f, target.y, 0f) - transform.position).sqrMagnitude)
                {
                    agent.destination = new Vector3(target.x - 0.75f, target.y, 0f);
                }
                else
                {
                    agent.destination = new Vector3(target.x + 0.75f, target.y, 0f);
                }

                //Orientation droite ou gauche en fonction de la direction
                FaceTarget(target);

                isMoving = true;
                movingTimer = Time.time;
            }
        }
        return (isMoving, movingTimer);
    }

    
}