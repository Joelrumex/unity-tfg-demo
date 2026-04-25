using UnityEngine;
public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float fallMultiplier = 3f;
    [SerializeField] private float lowJumpMultiplier = 6f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckRadius = 0.1f;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        // Detectar contacto con pared
        bool touchingLeft  = Physics2D.OverlapCircle(wallCheckLeft.position,  wallCheckRadius, wallLayer);
        bool touchingRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        isTouchingWall = !isGrounded &&
                         ((touchingLeft  && moveInput < 0) ||
                          (touchingRight && moveInput > 0));

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Si está pegado a la pared, no aplicar fuerza horizontal
        if (!isTouchingWall)
        {
            float targetSpeed = moveInput * moveSpeed;
            float speedDiff   = targetSpeed - rb.linearVelocity.x;
            float accelRate   = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            rb.AddForce(speedDiff * accelRate * Vector2.right);
        }

        // Gravedad variable
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.W))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }
}