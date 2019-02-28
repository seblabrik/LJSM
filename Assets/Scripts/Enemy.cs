using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private float scale = 1f;
    private Animator animator;
    private Collider2D enemyCollider;
    public float rangeAttack = 0.7f;

    private float meleeRange = 0.7f;
    private float damage = 10f;
    private float hp = 20f;
    private float timer;
    private float attackSpeed = 1f;

    private float aggroRange = 5f;
    private GameObject player;

    private float apFull = 75f;
    private float apAttackCost = 50f;
    private float apMovingCost = 25f;
    private float ap;
    private bool isMoving = false;
    private float movingTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        //2 modifs pour éviter de bugs du NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        timer = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (GameManager.instance.fightMode)
        {
            if (!GameManager.instance.playerTurn)
            {
                agent.isStopped = false;

                if (ap < 0.1f)
                {
                    isMoving = false;
                    GameManager.instance.playerTurn = true;
                }
                else
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
                            Attack(hit);
                            ap = Math.Max(0f, ap - apAttackCost);
                            break;
                        }
                    }
                    if (!canAttack)
                    {
                        if (isMoving)
                        {
                            ap = Math.Max(0f, ap - (Time.time - movingTimer) * apMovingCost);
                            movingTimer = Time.time;
                            if (HasReachedDestination()) { isMoving = false; }
                        }
                        else
                        {
                            //Fonction déplacement
                            agent.destination = target;

                            //Orientation droite ou gauche en fonction de la direction
                            bool isFacingRight = (transform.localScale.x >= 0);
                            bool isGoingRight = (transform.position.x <= target.x);
                            if (!(isFacingRight && !isGoingRight) && !(!isFacingRight && isGoingRight))
                            {
                                transform.localScale += new Vector3((-2) * scale, 0f, 0f);
                                scale = transform.localScale.x;
                            }

                            isMoving = true;
                            movingTimer = Time.time;
                        }
                    }
                }
            }
            else
            {
                agent.isStopped = true;
                agent.ResetPath();

                ap = apFull;
            }
        }
        else
        {
            float sqrDist = (player.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < aggroRange * aggroRange) { GameManager.instance.EnterFightMode(); }
            ap = apFull;
        }
    }

    private void Attack(RaycastHit2D hit)
    {
        if (hit.distance <= rangeAttack && Time.time - timer > attackSpeed)
        {
            animator.SetTrigger("EnemyAttack");
            if (hit.distance <= meleeRange)
            {
                hit.transform.SendMessage("GetHit", damage, SendMessageOptions.DontRequireReceiver);
            }
            timer = Time.time;
        }
    }

    private void GetHit(float damage)
    {
        hp -= damage;
        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if (hp <= 0f)
        {
            GameManager.instance.ExitFightMode();
            Destroy(gameObject);
        }
    }

    private bool HasReachedDestination()
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
}
