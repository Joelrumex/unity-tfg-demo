using System.Collections;
using UnityEngine;

public class ShieldGhost : MonoBehaviour
{
    [Header("Size Control")]
    public float baseSize = 1f;        // Master size — adjust this in Inspector first
                                       // until it looks right at scale (1,1)

    private SpriteRenderer _sr;
    private Transform _spriteChild;

    void Awake()
    {
        var child = new GameObject("GhostSprite");
        child.transform.SetParent(transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = Vector3.one;

        _sr = child.AddComponent<SpriteRenderer>();
        _sr.sortingLayerName = "Units";
        _sr.sortingOrder = 10;
        _sr.color = new Color(0.6f, 0.85f, 1f, 0f);

        _spriteChild = child.transform;
    }

    public void Appear(Sprite sprite, Vector3 playerPosition, Vector2 scale)
    {
        if (sprite == null) { Debug.LogWarning("ShieldGhost: sprite is null."); return; }
        if (scale == Vector2.zero) { scale = Vector2.one; }

        _sr.sprite = sprite;

        // Normalize size using pixels per unit
        float normalizedSize = 100f / sprite.pixelsPerUnit;
        float finalX = normalizedSize * baseSize * scale.x;
        float finalY = normalizedSize * baseSize * scale.y;

        // Apply scale FIRST
        _spriteChild.localScale = new Vector3(-finalX, finalY, 1f);

        // Then calculate the center offset from the sprite's pivot
        // Sprite pivot is in normalized coords (0-1), we need to
        // convert it to world space and subtract to center the sprite
        Vector2 pivot = sprite.pivot / sprite.rect.size;   // normalized pivot (0-1)
        Vector2 pivotOffset = pivot - new Vector2(0.5f, 0.5f);   // offset from center

        // Convert pivot offset to world units using final scale
        Vector3 correction = new Vector3(
            pivotOffset.x * finalX,
            pivotOffset.y * finalY,
            0f
        );

        // Position parent at player, then correct for pivot so sprite is centered
        transform.position = playerPosition - correction;

        Debug.Log($"Ghost pivot: {pivot}, correction: {correction}, finalScale: ({finalX:F2},{finalY:F2})");

        StopAllCoroutines();
        StartCoroutine(GhostAnimation());
    }

    IEnumerator GhostAnimation()
    {
        yield return StartCoroutine(Fade(0f, 0.7f, 0.15f));

        float elapsed = 0f;
        while (elapsed < 0.6f)
        {
            float alpha = 0.7f + Mathf.Sin(elapsed * 20f) * 0.15f;
            SetAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine(Fade(0.7f, 0f, 0.4f));
        _sr.sprite = null;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float alpha)
    {
        Color c = _sr.color;
        c.a = alpha;
        _sr.color = c;
    }
}