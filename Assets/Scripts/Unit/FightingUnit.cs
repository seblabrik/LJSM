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
    private bool playTurn_isActive = false;

    private BoxCollider2D boxCollider;

    private Vector3 moveTarget;

    public Gear gear;

    public Sprite spriteLeft;
    public Sprite spriteRight;
    public Sprite spriteUp;
    public Sprite spriteDown;

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
            item.gameObject = Instantiate(item.prefab, transform);
            EquipItem(item);
        }

        moveTarget = Vector3.zero;
    }

    protected virtual void InitTurn()
    {
        ap = unitStat.apFull;
        agent.isStopped = false;
        playTurn = PlayTurn();
        StartCoroutine(playTurn);
        playTurn_isActive = true;
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
        while (GameObject.FindGameObjectWithTag("Projectile") != null) { yield return new WaitForSeconds(0.1f); }//we wait for the projectiles to die before ending the turn
        yield return new WaitForSeconds(1);
        EndTurn();
    }

    protected abstract (bool, float) DoActionInTurn(bool isMoving, float movingTimer);

    protected void EndTurn()
    {
        agent.isStopped = true;
        agent.ResetPath();
        StopCoroutine(playTurn);
        playTurn_isActive = false;
        GameManager.instance.ChangeTurn();
    }

    protected void MeleeAttack(Vector3 target)
    {
        if (Time.time - attackTimer > unitStat.attackSpeed)
        {
            animator.SetTrigger(unitAnimation.meleeAttackAnimation);
            for (int i = 0; i< transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Animator>().SetTrigger(unitAnimation.meleeAttackAnimation);
            }
            ap = Math.Max(0f, ap - unitStat.apMeleeAttackCost);
            boxCollider.enabled = false;
            RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, target);
            boxCollider.enabled = true;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.distance <= unitStat.meleeRange && hit.transform != transform && hit.collider.tag != "GearItem")
                {
                    float dmg = unitStat.damage;
                    if (gear != null) { dmg += gear.getDamageBonus(); }
                    hit.transform.SendMessage("GetHit", dmg, SendMessageOptions.DontRequireReceiver);
                }
            }
            attackTimer = Time.time;
        }
    }

    protected void RangeAttack(Vector3 target)
    {
        if (Time.time - attackTimer > unitStat.attackSpeed)
        {
            animator.SetTrigger(unitAnimation.rangeAttackAnimation);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Animator>().SetTrigger(unitAnimation.rangeAttackAnimation);
            }
            ap = Math.Max(0f, ap - unitStat.apRangeAttackCost);

            GameObject projInst;
            foreach (GearItem item in gear.getItems())//provisoire
            {
                if (item.projectile != null)
                {
                    Quaternion rot = Quaternion.LookRotation(target - transform.position);
                    rot.Set(0, 0, rot.z, rot.w);
                    projInst = Instantiate(item.projectile.prefab, transform.position, rot);
                    if ((target - transform.position).x < float.Epsilon) { projInst.transform.localScale = new Vector3(-projInst.transform.localScale.x, projInst.transform.localScale.y); }
                    projInst.transform.SendMessage("SetParam", item.projectile);
                    projInst.transform.SendMessage("StraightFire", target);
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

    public virtual void EquipItem(GearItem item)
    {
        GameObject inst = Instantiate(item.gameObject, Vector3.zero, Quaternion.identity);
        if (item.color != null) { inst.GetComponent<SpriteRenderer>().color = item.color; }
        if (inst.transform.localScale.x * transform.localScale.x < 0) { inst.transform.localScale += new Vector3((-2) * inst.transform.localScale.x, 0f, 0f); }
        inst.transform.SetParent(transform);
        inst.transform.localPosition = Vector3.zero;
        GearItem newItem = item.Clone();
        newItem.gameObject = inst;
        newItem.owner = gameObject;
        //GearItem newItem = new GearItem { prefab = item.prefab, color = item.color, damage = item.damage, slot = item.slot, gameObject = inst, owner = gameObject };
        inst.transform.SendMessage("SetGearItem", newItem);
        gear.setItem(newItem);
        newItem.gameObject.transform.SendMessage("SetGearItem", newItem);
        if (item.gameObject != item.prefab) { Destroy(item.gameObject); }
    }

    public void SaveParams()
    {
        GameManager.instance.SaveUnitsParams(unitId, unitStat, gear, transform.position);
    }

    public void UnequipItem(GearItem item)
    {
        GameObject inst = Instantiate(item.gameObject, transform.position, Quaternion.identity, GameObject.Find("RoomObjects").transform);
        GearItem newItem = item.Clone();
        newItem.gameObject = inst;
        newItem.owner = null;
        inst.transform.SendMessage("SetGearItem", newItem);
        gear.removeItem(item.slot);
        Destroy(item.gameObject);
    }

    public void TakeItem()
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        Vector2 direction;
        if (transform.localScale.x >= 0) { direction = Vector2.right; }
        else { direction = Vector2.left; }
        GetComponent<BoxCollider2D>().Cast(direction, hits);
        RaycastHit2D hit = hits[0];
        if (hit.collider.tag == "GearItem")
        {
            GearItem item = hit.collider.GetComponent<GearItemScript>().gearItem;
            if (item.owner == null && gear.getItem(item.slot) == null)
            {
                EquipItem(item);
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Projectile" && !playTurn_isActive)
        {
            Projectile projectileParam = collider.GetComponent<ProjectileController>().projectileParam;
            GetHit(projectileParam.damage);
            Destroy(collider.gameObject);
        }
    }

    public void UpdateDirection()
    {
        Vector3 target = agent.steeringTarget;

        Vector3 position = transform.position;

        if ((target - position).sqrMagnitude < Mathf.Epsilon) { target = agent.destination; }

        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;

        float Dx = target.x - position.x;
        float Dy = target.y - position.y;

        if (Dy >= -Dx && Dy > Dx) { up = true; }
        else if (Dy < -Dx && Dy <= Dx) { down = true; }

        if (Dx >= 0) { right = true; }
        else { left = true; }
        
        SetAnimatorBool("left", left);
        SetAnimatorBool("right", right);
        SetAnimatorBool("up", up);
        SetAnimatorBool("down", down);

        //UpdateSprite(left, right, up, down);
    }

    private void UpdateSprite(bool left, bool right, bool up, bool down)
    {
        SpriteRenderer spriteR = gameObject.GetComponent<SpriteRenderer>();
        
        if (right && unitAnimation.spriteRight != null)
        {
            spriteR.sprite = unitAnimation.spriteRight;
        }
        else if (up && unitAnimation.spriteUp != null)
        {
            spriteR.sprite = unitAnimation.spriteUp;
        }
        else if (down && unitAnimation.spriteDown != null)
        {
            spriteR.sprite = unitAnimation.spriteDown;
        }
        else if (left && unitAnimation.spriteLeft != null)
        {
            spriteR.sprite = unitAnimation.spriteLeft;
        }
    }

    public void UpdateLateralOrientation()
    {
        if (animator.GetBool("right") && transform.localScale.x > 0)
        {
            transform.localScale += new Vector3((-2) * scale, 0f, 0f);
            scale = transform.localScale.x;
        }
        else if (animator.GetBool("left") && transform.localScale.x < 0)
        {
            transform.localScale += new Vector3((-2) * scale, 0f, 0f);
            scale = transform.localScale.x;
        }
    }

    protected void SetAnimatorBool(string name, bool value)
    {
        animator.SetBool(name, value);
        GameManager.instance.SavePlayerAnimatorParameter(name, value);
    }
}
