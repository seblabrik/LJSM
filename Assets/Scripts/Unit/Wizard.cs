using LJSM.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : FightingUnit
{
    private float moveTimer;
    private float castTimer;

    private float movingProba = 0.01f;
    private float castProba = 0.001f;

    private float invMovingProba;
    private float invCastProba;

    private float movingRange = 2;
    private float maxMovingTime = 2;//temps à partir duquel on arrête le déplacement (au cas où il se coince)

    bool isMoving = false;

    protected override void Start()
    {
        base.Start();

        unitAnimation = new UnitAnimation
        {
            SpriteFaceRight = true,
            castAnimation = "WizardCast",
            walkAnimation = "WizardWalk",
            stopWalkAnimation = "WizardStopWalk",
            fourDirections = false
        };

        moveTimer = Time.time;
        castTimer = Time.time;

        invMovingProba = 1/ movingProba;
        invCastProba = 1/ castProba;
    }


    void Update()
    {
        if (!GameManager.instance.pauseGame && !GameManager.instance.map.activeSelf && !isMoving && HasReachedDestination())
        {
            float m = Random.Range(0, invMovingProba);
            float c = Random.Range(0, invCastProba);

            if ((Time.time - moveTimer) >= m) { Move(); }
            if ((Time.time - castTimer) >= c) { Cast(); }
        }

        else if (isMoving && (HasReachedDestination() || (Time.time - moveTimer) > maxMovingTime))
        {
            isMoving = false;
            animator.SetTrigger(unitAnimation.stopWalkAnimation);
        }
    }

    private void Move()
    {
        isMoving = true;

        float distance = Random.Range(0, movingRange);

        float x = Random.Range(-1 * distance, distance);
        float y = Random.Range(-1 * distance, distance);

        Vector3 target = new Vector3(x, y, 0f);
        target = target + transform.position;

        FaceTarget(target);

        agent.destination = target;

        animator.SetTrigger(unitAnimation.walkAnimation);

        moveTimer = Time.time;
    }

    private void Cast()
    {
        animator.SetTrigger(unitAnimation.castAnimation);
        castTimer = Time.time;
    }

    protected override (bool, float) DoActionInTurn(bool isMoving, float movingTimer)
    {
        throw new System.NotImplementedException();
    }

    protected override void GetHit(float damage)
    {
        throw new System.NotImplementedException();
    }

    protected override void IsDead()
    {
        throw new System.NotImplementedException();
    }
}
