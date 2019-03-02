using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private float scale = 1f;
    private Animator animator;
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

    private IEnumerator playTurn;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        //2 modifs pour éviter de bugs du NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        timer = Time.time;
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (!GameManager.instance.fightMode)
        {
            float sqrDist = (player.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < aggroRange * aggroRange) { GameManager.instance.EnterFightMode(); }
            ap = apFull;
        }
    }

    public void InitTurn()
    {
        ap = apFull;
        agent.isStopped = false;
        playTurn = PlayTurn();
        StartCoroutine(playTurn);
    }

    private IEnumerator PlayTurn()
    {
        bool isMoving = false;
        while (ap >= 0.1f)
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

            yield return null;
        }
        yield return new WaitForSeconds(1);
        EndTurn();
    }

    private void EndTurn()
    {
        agent.isStopped = true;
        agent.ResetPath();
        StopCoroutine(playTurn);
        GameManager.instance.ChangeTurn();
    }

    
    private void Attack(RaycastHit2D hit)
    {
        if (hit.distance <= rangeAttack && Time.time - timer > attackSpeed)
        {
            animator.SetTrigger("EnemyAttack");
            ap = Math.Max(0f, ap - apAttackCost);
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
            GameManager.instance.HasDied(transform);
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

    private void FaceTarget(Vector3 target)
    {
        bool isFacingRight = (transform.localScale.x >= 0);
        bool isGoingRight = (transform.position.x <= target.x);
        if (!(isFacingRight && !isGoingRight) && !(!isFacingRight && isGoingRight))
        {
            transform.localScale += new Vector3((-2) * scale, 0f, 0f);
            scale = transform.localScale.x;
        }
    }
}
