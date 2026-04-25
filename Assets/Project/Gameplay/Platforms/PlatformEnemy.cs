using System.Collections;
using UnityEngine;

public class PlatformEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectRange = 6f;
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float jumpForce = 5f;

    [Header("Combat")]
    public int damage = 10;
    public float damageCooldown = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool canDamage = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(RandomJumpRoutine());
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }

        UpdateAnimations();
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void Idle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    void UpdateAnimations()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("IsGrounded", isGrounded);
    }

    IEnumerator RandomJumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            if (isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Player") && canDamage)
        {
            canDamage = false;

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, transform.position);
            }

            StartCoroutine(DamageCooldown());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}