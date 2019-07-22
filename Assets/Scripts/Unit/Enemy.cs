using LJSM.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : FightingUnit
{
    protected float rangeAttack;
    protected float aggroRange;
    protected GameObject player;

    private void Awake()
    {
        unitAnimation = new UnitAnimation
        {
            SpriteFaceRight = false,
            meleeAttackAnimation = "melee",
            rangeAttackAnimation = "range",
            spriteDown = spriteDown,
            spriteLeft = spriteLeft,
            spriteRight = spriteRight,
            spriteUp = spriteUp,
            fourDirections = false
        };
    }

    protected override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (!GameManager.instance.fightMode)
        {
            float sqrDist = (player.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < aggroRange * aggroRange) { GameManager.instance.EnterFightMode(); }
            ap = unitStat.apFull;
        }
    }

    protected override abstract (bool, float) DoActionInTurn(bool isMoving, float movingTimer);
    

    protected override void GetHit(float damage)
    {
        animator.SetTrigger("hit");
        unitStat.hp -= damage;
        CheckIfDead();
    }

    protected override void IsDead()
    {
        foreach (GearItem item in gear.getItems()) { UnequipItem(item); }
        GameManager.instance.HasDied(transform);
        Destroy(gameObject);
    }

    public override void EquipItem(GearItem item)
    {
        base.EquipItem(item);
        gear.getItem(item.slot).gameObject.transform.localScale += new Vector3(2 * item.gameObject.transform.localScale.x, 0f, 0f);
    }
}