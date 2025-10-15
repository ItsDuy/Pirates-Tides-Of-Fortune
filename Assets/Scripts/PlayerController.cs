using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Jump Settings")]
    [SerializeField] private float minJumpForce = 3f; // Minimum jump force (instant tap)
    [SerializeField] private float maxJumpForce = 10f; // Maximum jump force (full charge)
    [SerializeField] private float maxChargeTime = 0.5f; // Maximum time to charge jump in seconds
    [SerializeField] private float fallGravityMultiplier = 2.5f; // Gravity multiplier when falling
    [SerializeField] private float lowJumpMultiplier = 2f; // Gravity multiplier for short jumps
    
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool isJumping;
    private float defaultGravityScale;
    private float jumpChargeTime; // Time the jump button has been held
    private bool isChargingJump; // Whether we're currently charging a jump
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        ApplyBetterJumpPhysics();
        UpdateAnimations();
    }

    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        animator.SetBool("isRunning", moveInput != 0);
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Start charging jump when space is pressed
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isChargingJump = true;
            jumpChargeTime = 0f;
        }
        
        // Accumulate charge time while holding space (up to max)
        if (isChargingJump && Input.GetKey(KeyCode.Space))
        {
            jumpChargeTime += Time.deltaTime;
            jumpChargeTime = Mathf.Min(jumpChargeTime, maxChargeTime); // Cap at max charge time
        }
        
        // Execute jump when space is released
        if (isChargingJump && Input.GetKeyUp(KeyCode.Space))
        {
            // Calculate jump force based on charge time (linear interpolation from min to max)
            float chargePercent = jumpChargeTime / maxChargeTime;
            float actualJumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, chargePercent);
            
            animator.SetTrigger("isJumping");
            rb.velocity = new Vector2(rb.velocity.x, actualJumpForce);
            isJumping = true;
            isChargingJump = false;
            jumpChargeTime = 0f;
        }
        
        // Reset jumping state when grounded
        if (isGrounded)
        {
            isJumping = false;
        }
        
        // Cancel charge if player leaves ground without jumping
        if (!isGrounded && isChargingJump)
        {
            isChargingJump = false;
            jumpChargeTime = 0f;
        }
    }
    
    private void ApplyBetterJumpPhysics()
    {
        // Apply stronger gravity when falling
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = defaultGravityScale * fallGravityMultiplier;
        }
        // Apply increased gravity when ascending but button released (for short jumps)
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.gravityScale = defaultGravityScale * lowJumpMultiplier;
        }
        // Normal gravity when jumping and holding button
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }
    
    private void UpdateAnimations()
    {
        // Set falling animation when player is falling (moving downward in air)
        if (rb.velocity.y < -0.1f && !isGrounded)
        {
            animator.SetBool("isFalling", true);
        }
        
        // Reset falling animation only when player lands on ground
        if (isGrounded && animator.GetBool("isFalling"))
        {
            animator.SetBool("isFalling", false);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    
}
