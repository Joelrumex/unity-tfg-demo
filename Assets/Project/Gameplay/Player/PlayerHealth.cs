using System.Collections;
using UnityEngine;

using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public HealthBarUI healthBar;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    public float knockbackUpForce = 5f;

    [Header("Invulnerability")]
    public float invulnerabilityTime = 0.5f;
    private bool canTakeDamage = true;

    private SpriteRenderer spriteRenderer;

    [Header("Hit Flash")]
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;
    private Color originalColor;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Start()
    {
        healthBar.SetMaxHealth(health);
        health = PlayerManager.Instance.playerHealth;
    }

    public void TakeDamage(int amount, Vector2 damageSource)
    {
        if (!canTakeDamage) return;

        canTakeDamage = false;

        health -= amount;
        PlayerManager.Instance.playerHealth = health;
        healthBar.SetHealth(health);

        ApplyKnockback(damageSource);

        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(HitFlash());
        StartCoroutine(Invulnerability());
    }

    void ApplyKnockback(Vector2 sourcePosition)
    {
        if (rb == null) return;

        Vector2 direction = (transform.position - (Vector3)sourcePosition).normalized;
        Vector2 force = new Vector2(direction.x * knockbackForce, knockbackUpForce);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    IEnumerator Invulnerability()
    {
        yield return new WaitForSeconds(invulnerabilityTime);
        canTakeDamage = true;
    }

    IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Jugador muerto");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}