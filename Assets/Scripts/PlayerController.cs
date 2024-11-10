using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float accelerationInAir;
    [SerializeField] private float maxGroundedSpeed;
    [SerializeField] private float maxAirSpeed;
    [SerializeField] private float globalMaxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpSideWallForce;
    [SerializeField] private float wallSideForce;
    [SerializeField] private float wallUpForce;
    [SerializeField] private float slideForce;
    [SerializeField] private float slideCooldown;
    [SerializeField] private float slideDuration;
    [SerializeField] private float slideSizeReductionFactor;
    [SerializeField] private float dashForce;
    [SerializeField] private float playerHeight;
    [SerializeField] private float playerWidth;
    [SerializeField] private int jumpsCount;
    [SerializeField] private float decelerationForce;
    [SerializeField] private float slopeNormalForce;
    [SerializeField] private LayerMask groundCheckLayer;
    [SerializeField] private LayerMask wallCheckLayer;
    public TextMeshProUGUI speedCounterText;

    private Vector2 inputMovement = Vector2.zero;
    private Vector2 dashInputMovement = Vector2.zero;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    private int jumpsLeft;
    private bool grounded;

    private bool canSlide = true;
    private bool isSliding = false;

    private bool wallLeft;
    private bool wallRight;
    private bool exitingWall = false;
    private float enteringSpeed;

    private bool canDash;
    [SerializeField] private float dashCooldown;
    private float currentDashCooldown = 0;

    private bool onSlope;
    private bool exitingSlope = false;
    private Vector2 slopeNormal = Vector2.zero;

    public UnityEvent startTimer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
        Move();
        WallMove();


        // flip sprite so the player always face the wall
        if (wallLeft)
        {
            spriteRenderer.flipX = true;
        }
        else if (wallRight)
        {
            spriteRenderer.flipX = false;
        }


        /*if (inputMovement.x == 0 && grounded)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }*/

        if (currentDashCooldown > 0)
        {
            currentDashCooldown -= Time.deltaTime;
        }

        if (grounded && rb.velocity.magnitude >= 3)
        {
            startTimer.Invoke();
        }
    }

    public void OnMovePlayer(InputAction.CallbackContext context)
    {
        inputMovement = new Vector2(context.ReadValue<Vector2>().x, 0);
        if (inputMovement.x > 0)
        {
            inputMovement.x = 1;
            if (!wallLeft && !wallRight)
            {
                spriteRenderer.flipX = false;
            }
            animator.SetBool("running", true);
        }
        else if (inputMovement.x < 0)
        {
            inputMovement.x = -1;
            animator.SetBool("running", true);
            if (!wallLeft && !wallRight)
            {
                spriteRenderer.flipX = true;
            }
        }
        else
        {
            inputMovement.x = 0;
            animator.SetBool("running", false);
        }

        dashInputMovement = context.ReadValue<Vector2>();
    }

    private void Move()
    {
        Vector2 playerDirection;
        if (!isSliding)
        {
            if (grounded)
            {
                if (onSlope)
                {
                    rb.AddForce(-slopeNormal * slopeNormalForce, ForceMode2D.Force);
                    playerDirection.y = -Vector2.Perpendicular(slopeNormal).normalized.y; // we want to align the movement of the player with the slope but we want to keep the same speed on the x axis
                    playerDirection.x = inputMovement.x;
                }
                else
                {
                    playerDirection = inputMovement;
                }
                Debug.DrawRay(transform.position, playerDirection, Color.red);

                if (rb.velocity.x * rb.velocity.x <= maxGroundedSpeed * maxGroundedSpeed)
                {
                    //rb.AddForce(new Vector2(acceleration,0), ForceMode2D.Force);
                    rb.velocity = new Vector2(acceleration * playerDirection.x, rb.velocity.y);
                }
                else
                {
                    rb.AddForce(-playerDirection * decelerationForce, ForceMode2D.Force);

                    if (rb.velocity.x > 0)
                    {
                        if (inputMovement.x > 0)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                        }
                        else if (inputMovement.x < 0)
                        {
                            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(0, rb.velocity.y);
                        }
                    }
                    else
                    {
                        if (inputMovement.x >= 0)
                        {
                            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
                        }
                        else if (inputMovement.x < 0)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(0, rb.velocity.y);
                        }
                    }

                }
                animator.SetBool("grounded", true);
            }
            else
            {
                rb.AddForce(inputMovement * accelerationInAir, ForceMode2D.Force);
                animator.SetBool("grounded", false);
            }
        }

        if (rb.velocity.x * rb.velocity.x > globalMaxSpeed * globalMaxSpeed)
        {
            rb.velocity = new Vector2(globalMaxSpeed * (rb.velocity.x / Mathf.Abs(rb.velocity.x)), rb.velocity.y); // set speed to globalMaxSpeed and multiply by the good sign
        }

        speedCounterText.text = Mathf.Abs(rb.velocity.x).ToString("F2"); // counter of the speed x axis
        //speedCounterText.text = rb.velocity.magnitude.ToString("F2");  //counter of the speed either x or y axis
    }

    private void WallMove()
    {
        if (wallLeft && !exitingWall)
        {
            rb.gravityScale = 0;
            rb.AddForce(Vector2.left * wallSideForce, ForceMode2D.Force);
            rb.velocity = new Vector2(rb.velocity.x, wallUpForce);
            jumpsLeft = jumpsCount+1;
        }
        else
        {
            rb.gravityScale = 1;
        }

        if (wallRight && !exitingWall)
        {
            rb.gravityScale = 0;
            rb.AddForce(Vector2.right * wallSideForce, ForceMode2D.Force);
            rb.velocity = new Vector2(rb.velocity.x, wallUpForce);
            jumpsLeft = jumpsCount+1;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsLeft > 1 && context.started)
        {
            if (wallLeft)
            {
                exitingWall = true;
                Invoke("ResetExitingWall", 0.1f);
                wallLeft = false;
                rb.AddForce(Vector2.right * enteringSpeed, ForceMode2D.Impulse);
                /*if (dashInputMovement.x > 0)
                {
                    rb.AddForce(dashInputMovement * enteringSpeed, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce(Vector2.right * enteringSpeed, ForceMode2D.Impulse);
                }*/
            }
            else if (wallRight)
            {
                exitingWall = true;
                Invoke("ResetExitingWall", 0.1f);
                wallRight = false;
                rb.AddForce(Vector2.left * enteringSpeed, ForceMode2D.Impulse);
                /*if (dashInputMovement.x < 0)
                {
                    rb.AddForce(dashInputMovement * enteringSpeed, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce(Vector2.left * enteringSpeed, ForceMode2D.Impulse);
                }*/
            }
            onSlope = false;
            exitingSlope = true;
            Invoke("ResetExitingSlope", 0.1f);
            animator.SetTrigger("jumping");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpsLeft--;
        }
    }

    public void Slide(InputAction.CallbackContext context)
    {
        if (canSlide && context.started && grounded)
        {
            canSlide = false;
            isSliding = true;
            col.size = new Vector2(col.size.x, slideSizeReductionFactor * col.size.y);
            rb.AddForce(Vector2.down * 5, ForceMode2D.Impulse);
            Invoke("StopSlide", slideDuration);
            Invoke("ResetSlide", slideCooldown);
            rb.AddForce(inputMovement * slideForce, ForceMode2D.Impulse);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && canDash && !grounded && !wallLeft && !wallRight)
        {
            float momentum = Vector2.Dot(dashInputMovement.normalized, rb.velocity.normalized);
            if ( momentum < 0)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(dashInputMovement * dashForce, ForceMode2D.Impulse);
            }
            else
            {
                rb.velocity = rb.velocity * momentum;
                rb.AddForce(dashInputMovement * dashForce, ForceMode2D.Impulse);
            }
            canDash = false;
            currentDashCooldown = dashCooldown;
        }
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = new RaycastHit2D();
        bool hasHit = false;
        if (Physics2D.Raycast(transform.position, Vector2.down, playerHeight / 2 + 0.2f, groundCheckLayer))
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down, playerHeight / 2 + 0.2f, groundCheckLayer);
            hasHit = true;
        }
        else if (Physics2D.Raycast(transform.position - new Vector3(playerWidth / 2, 0, 0), Vector2.down, playerHeight / 2 + 0.2f, groundCheckLayer))
        {
            hit = Physics2D.Raycast(transform.position - new Vector3(playerWidth / 2, 0, 0), Vector2.down, playerHeight / 2 + 0.2f, groundCheckLayer);
            hasHit = true;
        }
        else if (Physics2D.Raycast(transform.position + new Vector3(playerWidth / 2, 0, 0), Vector2.down, playerHeight / 2 + 0.2f, groundCheckLayer))
        {
            hit = Physics2D.Raycast(transform.position + new Vector3(playerWidth / 2, 0, 0), Vector2.down, playerHeight / 2 + 0.2f, groundCheckLayer);
            hasHit = true;
        }

        if (hasHit)
        {
            grounded = true;
            jumpsLeft = jumpsCount;
            if (currentDashCooldown <= 0)
            {
                ResetDash();
            }

            if (hit.normal != Vector2.up && !exitingSlope)
            {
                onSlope = true;
                slopeNormal = hit.normal;
                Debug.Log("on slope");
            }
            else
            {
                onSlope = false;
            }
        }
        else
        {
            grounded = false;
        }
    }

    private void WallCheck()
    {
        if (Physics2D.Raycast(transform.position, Vector2.left, playerWidth/2 + 0.2f, wallCheckLayer) || Physics2D.Raycast(transform.position - new Vector3(0, playerHeight/2, 0), Vector2.left, playerWidth / 2 + 0.2f, wallCheckLayer) || Physics2D.Raycast(transform.position + new Vector3(0, playerHeight / 2, 0), Vector2.left, playerWidth / 2 + 0.2f, wallCheckLayer))
        {
            wallLeft = true;
            enteringSpeed = Mathf.Max(Mathf.Abs(rb.velocity.x), enteringSpeed);
        }
        else
        {
            wallLeft = false;
        }

        if (Physics2D.Raycast(transform.position, Vector2.right, playerWidth / 2 + 0.2f, wallCheckLayer) || Physics2D.Raycast(transform.position - new Vector3(0, playerHeight / 2, 0), Vector2.right, playerWidth / 2 + 0.2f, wallCheckLayer) || Physics2D.Raycast(transform.position + new Vector3(0, playerHeight / 2, 0), Vector2.right, playerWidth / 2 + 0.2f, wallCheckLayer))
        {
            wallRight = true;
            enteringSpeed = Mathf.Max(Mathf.Abs(rb.velocity.x), enteringSpeed);
        }
        else
        {
            wallRight = false;
        }

        if (!wallLeft && !wallRight)
        {
            exitingWall = true;
            animator.SetBool("wall", false);
            ResetExitingWall();
        }
        else
        {
            animator.SetBool("wall", true);
            if (currentDashCooldown <= 0)
            {
                ResetDash();
            }
        }
    }

    private void ResetExitingWall()
    {
        exitingWall = false;
        enteringSpeed = 5;
    }

    private void ResetExitingSlope()
    {
        exitingSlope = false;
    }


    private void StopSlide()
    {
        col.size = new Vector2(col.size.x, col.size.y / slideSizeReductionFactor);
        isSliding = false;
    }

    private void ResetSlide()
    {
        canSlide = true;
    }

    private void ResetDash()
    {
        canDash = true;
    } 

    public void Respawn(Vector3 position)
    {
        transform.position = position;
        rb.velocity = Vector2.zero;
    }
}
