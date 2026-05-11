using System.Collections;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    [Header("Sprite")]
    public Sprite slashSprite;
    public Sprite playerSlashSprite;
    public Color slashColor = Color.white;
    public bool flipX = false;
    public System.Action OnImpact;
    [Header("Settings")]
    public int slashCount = 3;
    public float duration = 0.3f;        // Travel time per slash
    public Vector2 baseScale = new Vector2(1.5f, 1.5f);
    public float scaleVariance = 0.2f;
    public float verticalSpread = 0.2f;       // Vertical offset between slashes

    void Awake()
    {
        for (int i = 0; i < slashCount; i++)
        {
            var go = new GameObject($"Slash_{i}");
            go.transform.SetParent(transform);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = slashSprite;
            sr.color = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
            sr.sortingLayerName = "Units";
            sr.sortingOrder = 5;
        }
    }

    // Now takes enemy AND player position
    public void Play(Vector3 from, Vector3 to)
    {
        StopAllCoroutines();
        SetSlashSprite(slashSprite);
        StartCoroutine(PlaySequence(from, to));
    }

    public void PlayAsPlayer(Vector3 from, Vector3 to)
    {
        StopAllCoroutines();
        SetSlashSprite(playerSlashSprite != null ? playerSlashSprite : slashSprite);
        StartCoroutine(PlaySequence(from, to));
    }

    // Applies the sprite to all pre-created children
    void SetSlashSprite(Sprite sprite)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var sr = transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = sprite;
        }
    }

    IEnumerator PlaySequence(Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < slashCount; i++)
        {
            var child = transform.GetChild(i);
            float delay = i * 0.07f;
            float vOffset = (i - slashCount / 2f) * verticalSpread;
            StartCoroutine(AnimateSlash(child, from + new Vector3(0, vOffset, 0),
                                              to + new Vector3(0, vOffset, 0), angle, delay));
        }

        // Fire impact callback when first slash arrives
        yield return new WaitForSeconds(duration);
        OnImpact?.Invoke();
    }

    IEnumerator AnimateSlash(Transform slashTransform, Vector3 from, Vector3 to, float angle, float delay)
    {
        var sr = slashTransform.GetComponent<SpriteRenderer>();

        // Wait for stagger delay
        yield return new WaitForSeconds(delay);
        sr.flipX = flipX;
        // Face the direction of travel
        slashTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Randomize scale slightly
        float scaleMultiplier = 1f + Random.Range(-scaleVariance, scaleVariance);
        slashTransform.localScale = new Vector3(
            baseScale.x * scaleMultiplier,
            baseScale.y * scaleMultiplier,
            1f
        );

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Move from enemy to player using smooth step
            slashTransform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));

            // Fade in first 20%, full opacity middle, fade out last 25%
            float alpha = t < 0.2f
                ? Mathf.Lerp(0f, 1f, t / 0.2f)
                : t > 0.75f
                    ? Mathf.Lerp(1f, 0f, (t - 0.75f) / 0.25f)
                    : 1f;

            // Scale punch on impact (last 25%)
            float punch = t > 0.75f
                ? 1f + Mathf.Sin((t - 0.75f) / 0.25f * Mathf.PI) * 0.3f
                : 1f;

            slashTransform.localScale = new Vector3(
                baseScale.x * scaleMultiplier * punch,
                baseScale.y * scaleMultiplier * punch,
                1f
            );

            sr.color = new Color(slashColor.r, slashColor.g, slashColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to player and reset
        slashTransform.position = to;
        sr.color = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
    }
}