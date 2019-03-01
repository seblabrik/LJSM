using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private float scale = 1f;
    private Animator animator;
    public float roomChangeDelay = 1f;

    private int width;
    private int height;

    private float meleeRange = 1f;
    private float damage = 10f;
    private float hp = 100f;
    private float timer;
    private float attackSpeed = 0.5f;

    private bool hasPlayed = false;

    private Text hpText;

    private float apFull = 100f;
    private float apAttackCost = 50f;
    private float apMovingCost = 25f;
    private float ap;
    private bool isMoving = false;
    private float movingTimer;

    private Text apText;

    private IEnumerator playTurn;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        //2 modifs pour éviter de bugs du NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponent<Animator>();

        width = GameManager.instance.width;
        height = GameManager.instance.height;

        timer = Time.time;

        hpText = GameObject.Find("hpText").GetComponent<Text>();
        hpText.text = "HP: " + Math.Floor(hp);

        apText = GameObject.Find("apText").GetComponent<Text>();
        apText.text = "";
    }


    void Update()
    {
        if (!GameManager.instance.pauseGame)
        {
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;

            if (GameManager.instance.fightMode)
            {
                enabled = false;
                agent.isStopped = true;
                agent.ResetPath();
                ap = apFull;
                apText.text = "AP: " + Math.Floor(ap);
            }

            else
            {
                //Orientation droite ou gauche en fonction de la position de la souris
                FaceTarget(target);
                
                if (Input.GetMouseButtonDown(0))
                {
                    agent.destination = target;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    Attack(target);
                }
            }
        }
    }

    public void InitTurn()
    {
        ap = apFull;
        apText.text = "AP: " + Math.Floor(ap);
        agent.isStopped = false;
        playTurn = PlayTurn();
        StartCoroutine(playTurn);
    }

    private IEnumerator PlayTurn()
    {
        bool isMoving = false;
        while (ap >= 0.1f)
        {
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;

            if (isMoving)
            {
                ap = Math.Max(0f, ap - (Time.time - movingTimer) * apMovingCost);
                apText.text = "AP: " + Math.Floor(ap);
                movingTimer = Time.time;
                if (HasReachedDestination()) { isMoving = false; }
            }
            else
            {

                //Orientation droite ou gauche en fonction de la position de la souris
                FaceTarget(target);

                if (Input.GetMouseButtonDown(0))
                {
                    agent.destination = target;
                    isMoving = true;
                    movingTimer = Time.time;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    Attack(target);
                    apText.text = "AP: " + Math.Floor(ap);
                }
            }
            if (!GameManager.instance.fightMode) { apText.text = ""; }
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

    private void ExitFightMode()
    {
        enabled = true;
        StopCoroutine(playTurn);//si la fight finit avant la fin du tour du Player
        apText.text = "";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", roomChangeDelay);
        }
    }

    private void Restart()
    {
        GameManager.instance.playerSpawn = GetNextSpawn(transform.position);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public string GetNextSpawn(Vector3 pos)
    {
        string playerSpawn = "Start";
        if (pos != Vector3.zero)//valeur par default
        {
            if (pos.y >= 3 * height / 4) { playerSpawn = "South"; }
            if (pos.y <= height / 4) { playerSpawn = "North"; }
            if (pos.x >= 3 * width / 4) { playerSpawn = "West"; }
            if (pos.x <= width / 4) { playerSpawn = "East"; }
        }
        return playerSpawn;
    }

    private void Attack(Vector3 target)
    {
        if (Time.time - timer > attackSpeed)
        {
            animator.SetTrigger("PlayerAttack");
            ap = Math.Max(0f, ap - apAttackCost);
            RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, target);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.distance <= meleeRange && hit.transform.tag != "Player")
                {
                    hit.transform.SendMessage("GetHit", damage, SendMessageOptions.DontRequireReceiver);
                }
            }
            timer = Time.time;
        }
    }

    private void GetHit(float damage)
    {
        animator.SetTrigger("PlayerHit");
        hp -= damage;
        hpText.text = "HP: " + Math.Floor(hp);
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (hp <= 0f)
        {
            GameManager.instance.GameOver();
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
        //Orientation droite ou gauche en fonction de la position de la souris
        bool isFacingRight = (transform.localScale.x >= 0);
        bool isGoingRight = (transform.position.x <= target.x);
        if ((isFacingRight && !isGoingRight) || (!isFacingRight && isGoingRight))
        {
            transform.localScale += new Vector3((-2) * scale, 0f, 0f);
            scale = transform.localScale.x;
        }
    }

}
