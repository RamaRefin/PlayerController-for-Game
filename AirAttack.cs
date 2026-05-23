using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class AirAttack : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement; 
    public Transform attackPoint; 
    public LayerMask enemyLayers; 

    [Header("Air Attack Settings")]
    public float airAttackHoverDuration = 0.5f; 
    public float airAttackCooldown = 0.7f; 
    public float airAttackDamage = 10f;
    public float attackRange = 0.5f; 

    private Rigidbody2D rb;
    private Animator animator;
    private bool isAirAttacking = false;
    private bool canAirAttack = true;
    private Coroutine airAttackCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                enabled = false; 
            }
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!playerMovement.isGrounded && canAirAttack)
            {
                PerformAirAttack();
            }
            else if (playerMovement.isGrounded)
            {
                
            }
            else if (!canAirAttack)
            {
                
            }
        }


        if (playerMovement.isGrounded && isAirAttacking)
        {
            Debug.Log("AirAttack.cs: Player landed while air attacking. Stopping air attack.");
            StopAirAttack();
        }
    }

    void PerformAirAttack()
    {
        isAirAttacking = true;
        canAirAttack = false;
        playerMovement.canMove = false; 

        rb.gravityScale = 0f; 
        rb.velocity = Vector2.zero; 

        animator.SetTrigger("AirAttack"); 

        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
           
        }

        airAttackCoroutine = StartCoroutine(AirAttackDurationRoutine());
    }

    IEnumerator AirAttackDurationRoutine()
    {
        yield return new WaitForSeconds(airAttackHoverDuration);
        StopAirAttack();
        StartCoroutine(AirAttackCooldownRoutine());
    }

    void StopAirAttack()
    {
        if (isAirAttacking)
        {
            isAirAttacking = false;
            playerMovement.canMove = true; 
            rb.gravityScale = 1f; 

        }
    }

    IEnumerator AirAttackCooldownRoutine()
    {
        yield return new WaitForSeconds(airAttackCooldown);
        canAirAttack = true;
        
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}