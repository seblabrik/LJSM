using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private float scale = 1f;
    private Animator animator;
    private Collider2D enemyCollider;
    public float rangeAttack = 0.5f;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        //2 modifs pour éviter de bugs du NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }


    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //Fonction déplacement
        var target = player.transform.position;
        target.z = 0;
        agent.destination = target;

        //Orientation droite ou gauche en fonction de la direction
        bool isFacingRight = (transform.localScale.x >= 0);
        bool isGoingRight = (transform.position.x <= target.x);
        if (!(isFacingRight && !isGoingRight) && !(!isFacingRight && isGoingRight))
        {
            transform.localScale += new Vector3((-2) * scale, 0f, 0f);
            scale = transform.localScale.x;
        }

        //Fonction d'attaque
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, target);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == "Player" && hit.distance <= rangeAttack)
            {
                animator.SetTrigger("Enemy1Attack");
            }
        }
    }
}
