using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private float scale = 1f;
    private Animator animator;
    public float roomChangeDelay = 1f;

    private int width;
    private int height;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        //2 modifs pour éviter de bugs du NavMesh
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = GetComponent<Animator>();

        width = GameManager.instance.width;
        height = GameManager.instance.height;
    }


    void Update()
    {
        if (!GameManager.instance.pauseGame)
        {
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;

            //Orientation droite ou gauche en fonction de la position de la souris
            bool isFacingRight = (transform.localScale.x >= 0);
            bool isGoingRight = (transform.position.x <= target.x);
            if ((isFacingRight && !isGoingRight) || (!isFacingRight && isGoingRight))
            {
                transform.localScale += new Vector3((-2) * scale, 0f, 0f);
                scale = transform.localScale.x;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //Fonction déplacement
                agent.destination = target;
            }

            if (Input.GetButtonDown("MeleeAttack"))
            //touche 'a' actuellement
            //cf Edit/ProjectSettings/Input/Axis/MeleeAttack pour modifier
            {
                animator.SetTrigger("PlayerAttack");
            }
        }
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
}
