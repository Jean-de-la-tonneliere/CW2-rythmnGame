using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -100f;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private int lastWallJumpDirection;
    private int livesCount = 0;
    
    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private bool isDashing;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;

    private Rigidbody2D rb;
    private Animator anim;

    private static bool teoIsANoob = false;
    private bool wallJumpNoInput = false;
    private int wallJumpMiniDelay = 0;

    public int amountOfJumps = 1;

    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet = 0.15f;
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;
    public float dashTime = 0.2f;
    public float dashSpeed = 50f;
    public float distanceBetweenImages = 0.1f;
    public float dashCoolDown = 2.5f;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck;

    public LayerMask whatIsGround;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    private void ResetAnimTriggers()
    {
        //anim.ResetTrigger("jump");
        //anim.ResetTrigger("Attack");
        //anim.ResetTrigger("Damage");
    }

    private void Update()
    {
        ResetAnimTriggers();
        CheckInput();
        CheckMovementDirection();
        CheckIfCanJump();
        CheckIfWallSliding();
        //CheckLedgeClime(); needs a ledge climb animation
        CheckJump();
        CheckDash();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurrounding();
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && movementInputDirection == facingDirection && rb.linearVelocityY < 0 && !canClimbLedge)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    public static void toggleTeoIsANoobMode()
    {
        teoIsANoob = !teoIsANoob;
    }

    private void CheckLedgeClime()
    {
        if(ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Ceil(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Ceil(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            canMove = false;
            canFlip = false;

            //anim.SetBool("canClimbLedge", canClimbLedge); when animations are added. uncomment this
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    public void FinishLedgeClimb() //this is used when the animation finishes
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        //anim.SetBool("canClimbLedge", canClimbLedge); when animations are added. uncomment this
    }

    private void CheckSurrounding() 
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    private void CheckIfCanJump()
    {
        if(isGrounded && rb.linearVelocityY <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }
    private void CheckMovementDirection()
    {
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.linearVelocityX) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimations() 
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isStanding", !isWalking);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isJumpingUp", jumpTimer > 0 && isGrounded);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocityY);
    }

    private void CheckInput()
    {
        if (!wallJumpNoInput)
        {
            movementInputDirection = Input.GetAxisRaw("Horizontal");
        }
        else if (wallJumpNoInput && wallJumpMiniDelay <= 0 && (isGrounded || isTouchingWall))
        {
            wallJumpNoInput = false;
            //wallJumpMiniDelay = 10;
        }
        wallJumpMiniDelay--;



        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }


        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * variableJumpHeightMultiplier);
        }

        if (Input.GetButtonDown("Dash"))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            AttemptToDash();
        }

        if (Input.GetButtonDown("Attack"))
        {
            anim.SetTrigger("Attack");
            Debug.Log("ATTACK");
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.linearVelocity = new Vector2(dashSpeed * facingDirection, 0f);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }

    private void CheckJump()
    {
        if(jumpTimer > 0)
        {
            //wallJump
            bool walljump = !isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection;
            if (teoIsANoob)
            {
                walljump = !isGrounded && isTouchingWall && movementInputDirection != 0;
            }
            if (walljump)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        }

        if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if(lastWallJumpDirection > 0)
        {
            if(hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, 0.0f);
                hasWallJumped = false;
            } else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
            amountOfJumpsLeft--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            anim.SetTrigger("jump");
        }
    }

    private void WallJump()
    {
        if (canWallJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0.0f);
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;

            if (teoIsANoob && movementInputDirection == facingDirection)
            {
                movementInputDirection *= -1f;
                wallJumpNoInput = true;
                wallJumpMiniDelay = 10;
            }

            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);

            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;

            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
            anim.SetTrigger("jump");
        }
    }
    private void ApplyMovement()
    {
        if (!isGrounded && !isWallSliding && movementInputDirection == 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX * airDragMultiplier, rb.linearVelocityY);
        }
        else if(canMove)
        {
            rb.linearVelocity = new Vector2(movementSpeed * movementInputDirection, rb.linearVelocityY);
        }

        if (isWallSliding)
        {
            if(rb.linearVelocityY < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if(!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    public void AddLife()
    {
        if (livesCount < 3)
        {
            livesCount++;
            GameObject.Find("Life indicator " + livesCount).GetComponent<UnityEngine.UI.Image>().enabled = true;
        }

    }
    public void Damage()
    {
        if (livesCount > 0)
        {
            GameObject.Find("Life indicator " + livesCount).GetComponent<UnityEngine.UI.Image>().enabled = false;
            livesCount--;
            anim.SetTrigger("Damage");
            rb.AddForce(new Vector2 (-25f, 20f), ForceMode2D.Impulse);
        }


    }

    public int getLivesCount()
    {
        return this.livesCount;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Life"))
        {
            AddLife();
            Destroy(other.gameObject);
        }
        
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            Damage();
            Destroy(other.gameObject);
        }
    }
}
