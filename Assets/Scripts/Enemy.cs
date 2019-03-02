using LJSM.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : FightingUnit
{
    public float rangeAttack = 0.7f;
    public float aggroRange = 5f;
    private GameObject player;

    private void Awake()
    {
        unitStat = new FightingUnitStat
        {
            meleeRange = 0.7f,
            damage = 10f,
            hp = 20f,
            attackSpeed = 1f,
            apFull = 75f,
            apAttackCost = 50f,
            apMovingCost = 25f
        };
        unitAnimation = new UnitAnimation
        {
            SpriteFaceRight = false,
            meleeAttackAnimation = "EnemyAttack"
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
                Attack(target);
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

    protected override void GetHit(float damage)
    {
        unitStat.hp -= damage;
        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if (unitStat.hp <= 0f)
        {
            GameManager.instance.HasDied(transform);
            Destroy(gameObject);
        }
    }
}