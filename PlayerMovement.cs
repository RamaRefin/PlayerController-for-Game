using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float jumpforce = 10f;

    [Header("Slide Settings")]
    public float slideSpeed = 5f;
    public float slideDuration = 0.8f;

    [Header("Crouching Settings")]
    public float crouchSpeed = 4f;
    public float crouchScaleY = 0.5f;

    [Header("Dodge Setting")]
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 1f;

    private bool isDodging = false;
    private bool canDodge = true;

    private bool isCrouching = false;
    private BoxCollider2D bodyCollider;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Vector3 originalScale;

    public bool canMove = true;
    public bool isGrounded = false;
    public bool isSlide = false;

    private Animator anim;
    private Rigidbody2D rb;
    public Collider2D regularCollider;
    public Collider2D slideCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalScale = transform.localScale;
        bodyCollider = GetComponent<BoxCollider2D>();

        if (bodyCollider != null)
        {
            originalColliderSize = bodyCollider.size;
            originalColliderOffset = bodyCollider.offset;
        }
    }


    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = canMove ? Input.GetAxis("Horizontal") : 0f;

       if(!isSlide && !isDodging)
       {
         rb.velocity = new Vector2(moveHorizontal * speed, rb.velocity.y);
       }
 

    //JumpPlayer
    if (canMove && Input.GetButtonDown("Jump") && isGrounded)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        isGrounded = false;
        anim.SetBool("isJumping", !isGrounded);
    }

    //flip karakter
    Vector3 scale = transform.localScale;

    if(canMove && moveHorizontal > 0)
    {
        scale.x = -1;
    } 
    else if (canMove && moveHorizontal < 0)
    {
        scale.x = 1;
        anim.SetBool("isJumping", Mathf.Abs(rb.velocity.y) > 0.1f);

    }


    // Slide player Input
    if(Input.GetKeyDown(KeyCode.C))
    {
        performSlide();
    }

    // Crouch player input
    bool crouchInput = Input.GetKey(KeyCode.S);
    if(crouchInput)
    {
        StartCrouch();
    } else {
        StopCrouch();
    }

    //dodge player input
    if(Input.GetKeyDown(KeyCode.LeftShift) && canDodge && !isDodging && isGrounded)
    {
        StartCoroutine(Dodge());
    }

    transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isGrounded = true;
        anim.SetBool("isJumping", !isGrounded);
    }

    private void FixedUpdate()
    {
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yVelocity", rb.velocity.y);
        float currentSpeed = isCrouching ? crouchSpeed : speed;
    }

    private void performSlide()
    {
        if (isSlide || !isGrounded || Mathf.Abs(rb.velocity.x) < 0.5f)
        {
            return;
        }

        isSlide = true;
        canMove = false;

        anim.SetBool("isSlide", true);

        regularCollider.enabled = false;
        slideCollider.enabled = true;

        float direction;

        if (transform.localScale.x > 0)
        {
            direction = -1f;
        }
        else
        {
            direction = 1f;
        }

        rb.velocity = new Vector2(direction * slideSpeed, rb.velocity.y);

        StartCoroutine(stopSlide());
    }

   IEnumerator stopSlide()
    {
        yield return new WaitForSeconds(slideDuration);

        anim.SetBool("isSlide", false);

        regularCollider.enabled = true;
        slideCollider.enabled = false;

        canMove = true;
        isSlide = false;
    }

    private void StartCrouch()
    {
        if (isCrouching) return;

        isCrouching = true;

        if (bodyCollider != null)
        {
            bodyCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * crouchScaleY);
            bodyCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - 0.25f);
        }

        transform.localScale = new Vector3(originalScale.x, originalScale.y * crouchScaleY, originalScale.z);
        anim.SetBool("isCrouching", true);
    }

    private void StopCrouch()
    {
        if (!isCrouching) return;

        isCrouching = false;

        if (bodyCollider != null)
        {
            bodyCollider.size = originalColliderSize;
            bodyCollider.offset = originalColliderOffset;
        }

        transform.localScale = originalScale;
        anim.SetBool("isCrouching", false);
    }

    IEnumerator Dodge()
    {
        isDodging = true;
        canDodge = false;
        canMove = false;

        anim.SetBool("isDodging", true);

        float direction;

        if (transform.localScale.x > 0)
        {
            direction = 1f;
        }
        else
        {
            direction = -1f;
        }

        rb.velocity = new Vector2(direction * dodgeSpeed, rb.velocity.y);

        yield return new WaitForSeconds(dodgeDuration);

        anim.SetBool("isDodging", false);
        canMove = true;
        isDodging = false;

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }

    private void SetAnimation(float moveHorizontal)
    {
      if (isCrouching)
      {
                anim.Play("Crouch");
        return;
      }
    }
}

