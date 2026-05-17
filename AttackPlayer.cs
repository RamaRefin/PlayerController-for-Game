using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    private GameObject attackArea = default;
    private bool attacking = false;
    public float cooldown = 2f;
    private float timer = 0f;
    public Animator anim;
    private PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        attackArea= transform.GetChild(0).gameObject;
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Attack();
        }

       if(timer > 0)
       {
            timer -= Time.deltaTime;

            if (timer <= 0 && attacking)
            {
                attacking = false;
                attackArea.SetActive(false);

                if (playerMovement != null)
                {
                    playerMovement.canMove = true;
                }
            }
       }
    }

    private void Attack()
    {
        if(timer > 0)
        {
            return;
        }

        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        anim.SetBool("isAttacking", true);
        timer = cooldown;

        attacking = true;
        attackArea.SetActive(true);
        
    }

    public void FinishAttack()
    {
        anim.SetBool("isAttacking", false);
        attackArea.SetActive(false);
    }
}
