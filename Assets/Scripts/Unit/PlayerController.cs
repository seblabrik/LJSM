using LJSM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : FightingUnit
{
    public float roomChangeDelay = 1f;

    private int width;
    private int height;

    private Text hpText;

    private Text apText;

    protected override void Start()
    {
        base.Start();

        Sprite[] sheet = Resources.LoadAll<Sprite>("Images/New Mobs/Characters/chara1/chara1");
        foreach (Sprite sprite in sheet)
        {
            if (sprite.name == "chara1_1") { spriteDown = sprite; }
            if (sprite.name == "chara1_13") { spriteLeft = sprite; }
            if (sprite.name == "chara1_25") { spriteRight = sprite; }
            if (sprite.name == "chara1_37") { spriteUp = sprite; }
        }

        unitAnimation = new UnitAnimation
        {
            SpriteFaceRight = true,
            meleeAttackAnimation = "MeleeAttack",
            rangeAttackAnimation = "MeleeAttack",
            spriteDown = spriteDown,
            spriteLeft = spriteLeft,
            spriteRight = spriteRight,
            spriteUp = spriteUp
        };

        hpText = GameObject.Find("hpText").GetComponent<Text>();
        hpText.text = "HP: " + Math.Floor(unitStat.hp);

        apText = GameObject.Find("apText").GetComponent<Text>();
        apText.text = "";

        animator.SetBool("isMoving", false);

        SpriteRenderer spriteR = gameObject.GetComponent<SpriteRenderer>();
        spriteR.sprite = unitAnimation.spriteDown;
    }
    
    void Update()
    {
        if (!GameManager.instance.pauseGame && !GameManager.instance.map.activeSelf)
        {
            if (GameManager.instance.fightMode)
            {
                enabled = false;
                agent.isStopped = true;
                agent.ResetPath();
                ap = unitStat.apFull;
                apText.text = "AP: " + Math.Floor(ap);
            }
            else
            {
                var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.z = 0;


                if (HasReachedDestination()) { animator.SetBool("isMoving", false); }

                //Orientation droite ou gauche en fonction de la position de la souris
                //FaceTarget(target);

                if (Input.GetMouseButtonDown(0))
                {
                    agent.destination = target;
                    animator.SetBool("isMoving", true);
                    animator.SetTrigger("move");
                }
                if (Input.GetMouseButtonDown(1))
                {
                    //MeleeAttack(target);
                    //RangeAttack(target);
                }
                if (Input.GetKeyDown("r"))
                {
                    foreach (GearItem item in gear.getItems()) { UnequipItem(item); }
                }
                if (Input.GetKeyDown("a"))
                {
                    TakeItem();
                }
            }
        }
    }

    protected override void InitTurn()
    {
        base.InitTurn();
        apText.text = "AP: " + Math.Floor(ap);
    }

    protected override (bool, float) DoActionInTurn(bool isMoving, float movingTimer)
    {
        var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = 0;

        if (isMoving)
        {
            ap = Math.Max(0f, ap - (Time.time - movingTimer) * unitStat.apMovingCost);
            apText.text = "AP: " + Math.Floor(ap);
            movingTimer = Time.time;
            if (HasReachedDestination()) { isMoving = false; }
            if (ap<=0.1f) { isMoving = false; agent.isStopped = true; agent.ResetPath(); }
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
                if (gear !=null && gear.rightHand != null && gear.rightHand.projectile != null) { RangeAttack(target); }
                else { MeleeAttack(target); }
                apText.text = "AP: " + Math.Floor(ap);
            }
        }
        if (!GameManager.instance.fightMode) { apText.text = ""; }
        return (isMoving, movingTimer);
    }

    private void ExitFightMode()
    {
        enabled = true;
        if (playTurn != null)
        {
            StopCoroutine(playTurn);//si la fight finit avant la fin du tour du Player
        }
        apText.text = "";
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.tag == "Exit")
        {
            Invoke("Restart", roomChangeDelay);
        }
    }

    private void Restart()
    {
        GameManager.instance.playerSpawn = GetNextSpawn(transform.position);
        GameManager.instance.ChangeRoom();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public SpecificSpot GetNextSpawn(Vector3 pos)
    {
        SpecificSpot playerSpawn = SpecificSpot.Start;
        if (pos != Vector3.zero)//valeur par default
        {
            var height = GameManager.instance.height;
            var width = GameManager.instance.height;
            if (pos.y >= 3 * height / 4) { playerSpawn = SpecificSpot.South; }
            if (pos.y <= height / 4) { playerSpawn = SpecificSpot.North; }
            if (pos.x >= 3 * width / 4) { playerSpawn = SpecificSpot.West; }
            if (pos.x <= width / 4) { playerSpawn = SpecificSpot.East; }
        }
        return playerSpawn;
    }

    protected override void GetHit(float damage)
    {
        animator.SetTrigger("PlayerHit");
        unitStat.hp -= damage;
        hpText.text = "HP: " + Math.Floor(unitStat.hp);
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (unitStat.hp <= 0f)
        {
            GameManager.instance.GameOver();
        }
    }
}