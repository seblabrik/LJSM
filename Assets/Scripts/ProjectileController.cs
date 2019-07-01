using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LJSM.Models;

public class ProjectileController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    public Projectile projectileParam;
    private float speed = 5f;
    private float lifeTime = 5f;
    private float timer;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        timer = Time.time;
    }

    private void Update()
    {
        if (Time.time - timer > lifeTime) { Destroy(gameObject); }
    }

    public void SetParam(Projectile param)
    {
        projectileParam = param;
    }

    public void StraightFire(Vector3 target)
    {
        //StartCoroutine(MoveTo(target));
        ConstantForce2D force = GetComponent<ConstantForce2D>();
        force.force = speed * (target - transform.position).normalized;
    }

    //private IEnumerator MoveTo(Vector3 target)
    //{
    //    float sqrRemainingDistance = (transform.position - target).sqrMagnitude;

    //    while (sqrRemainingDistance > 0.1f)
    //    {
    //        Vector3 newPostion = Vector3.MoveTowards(rb2D.position, target, Time.deltaTime * speed);
    //        rb2D.MovePosition(newPostion);
    //        sqrRemainingDistance = (transform.position - target).sqrMagnitude;
    //        yield return null;
    //    }
    //    Destroy(gameObject);
    //}
}
