using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private float scale = 1f;
    private Animator animator;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        //2 modifs pour éviter de bugs du NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Fonction déplacement
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;
            agent.destination = target;

            //Orientation droite ou gauche en fonction de la direction
            bool isFacingRight = (transform.localScale.x >= 0);
            bool isGoingRight = (transform.position.x <= target.x);
            if ((isFacingRight && !isGoingRight) || (!isFacingRight && isGoingRight))
            {
                transform.localScale += new Vector3((-2) * scale, 0f, 0f);
                scale = transform.localScale.x;
            }

        }

        if (Input.GetButtonDown("MeleeAttack"))
            //touche 'a' actuellement
            //cf Edit/ProjectSettings/Input/Axis/MeleeAttack pour modifier
        {
            animator.SetTrigger("PlayerAttack");
        }
    }
}
