using LJSM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class FightingUnit : MonoBehaviour
{
    protected NavMeshAgent agent;
    private float scale = 1f;
    protected Animator animator;

    public int unitId;
    public FightingUnitStat unitStat;
    protected UnitAnimation unitAnimation;

    protected float attackTimer;

    protected float ap;

    protected IEnumerator playTurn;

    private BoxCollider2D boxCollider;

    public Gear gear;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        agent = GetComponent<NavMeshAgent>();

        //2 modifs pour éviter des bugs du NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponent<Animator>();

        attackTimer = Time.time;

        foreach (GearItem item in gear.getItems())
        {
            EquipItem(item);
        }
    }

    protected virtual void InitTurn()
    {
        ap = unitStat.apFull;
        agent.isStopped = false;
        playTurn = PlayTurn();
        StartCoroutine(playTurn);
    }

    protected IEnumerator PlayTurn()
    {
        bool isMoving = false;
        float movingTimer = Time.time;
        while (ap >= 0.1f)
        {
            (isMoving, movingTimer) = DoActionInTurn(isMoving, movingTimer);
            yield return null;
        }
        yield return new WaitForSeconds(1);
        EndTurn();
    }

    protected abstract (bool, float) DoActionInTurn(bool isMoving, float movingTimer);

    protected void EndTurn()
    {
        agent.isStopped = true;
        agent.ResetPath();
        StopCoroutine(playTurn);
        GameManager.instance.ChangeTurn();
    }

    protected void Attack(Vector3 target)
    {
        if (Time.time - attackTimer > unitStat.attackSpeed)
        {
            animator.SetTrigger(unitAnimation.meleeAttackAnimation);
            for (int i = 0; i< transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Animator>().SetTrigger(unitAnimation.meleeAttackAnimation);
            }
            ap = Math.Max(0f, ap - unitStat.apAttackCost);
            boxCollider.enabled = false;
            RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, target);
            boxCollider.enabled = true;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.distance <= unitStat.meleeRange)
                {
                    float dmg = unitStat.damage;
                    if (gear != null) { dmg += gear.getDamageBonus(); }
                    hit.transform.SendMessage("GetHit", dmg, SendMessageOptions.DontRequireReceiver);
                }
            }
            attackTimer = Time.time;
        }
    }

    protected abstract void GetHit(float damage);

    protected bool HasReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected void FaceTarget(Vector3 target)
    {
        //Orientation droite ou gauche en fonction de la position de la souris
        bool isFacingRight = (transform.localScale.x >= 0);
        bool isGoingRight = (transform.position.x <= target.x);

        if (unitAnimation.SpriteFaceRight)
        {
            if ((isFacingRight && !isGoingRight) || (!isFacingRight && isGoingRight))
            {
                transform.localScale += new Vector3((-2) * scale, 0f, 0f);
                scale = transform.localScale.x;
            }
        }
        else
        {
            if (!(isFacingRight && !isGoingRight) && !(!isFacingRight && isGoingRight))
            {
                transform.localScale += new Vector3((-2) * scale, 0f, 0f);
                scale = transform.localScale.x;
            }
        }
    }

    protected void SetParam(UnitParam param)
    {
        unitId = param.id;
        unitStat = param.stat;
        gear = param.gear;
    }

    public virtual GameObject EquipItem(GearItem item)
    {
        if (item.slot == Slot.rightHand) { gear.rightHand = item; }
        if (item.slot == Slot.leftHand) { gear.leftHand = item; }
        if (item.slot == Slot.head) { gear.head = item; }

        return Instantiate(item.gameObject, transform);
    }

    public void SaveParams()
    {
        GameManager.instance.SaveUnitsParams(unitId, unitStat, gear, transform.position);
    }
}
