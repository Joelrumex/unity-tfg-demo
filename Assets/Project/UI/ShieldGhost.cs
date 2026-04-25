using System.Collections;
using UnityEngine;

public class ShieldGhost : MonoBehaviour
{
    [Header("Settings")]
    public float scale = 1.0f;

    private SpriteRenderer _sr;
    private Transform _spriteChild;   // Child handles scale only

    void Awake()
    {
        // Create a child GameObject to hold the sprite
        // This way parent = position, child = scale
        var child = new GameObject("GhostSprite");
        child.transform.SetParent(transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale    = Vector3.one;

        _sr = child.AddComponent<SpriteRenderer>();
        _sr.sortingLayerName = "Units";
        _sr.sortingOrder     = 2;
        _sr.color = new Color(0.6f, 0.85f, 1f, 0f);   // Blue ghost tint, invisible

        _spriteChild = child.transform;
    }

    public void Appear(Sprite sprite, Vector3 playerPosition, Vector2 scale)
    {
        if (sprite == null)
        {
            Debug.LogWarning("ShieldGhost: sprite is null.");
            return;
        }

        if (scale == Vector2.zero)
        {
            Debug.LogWarning("ShieldGhost: scale is (0,0) — defaulting to (1,1).");
            scale = Vector2.one;
        }

        _sr.sprite = sprite;

        // Parent controls position only — never touched by scale
        transform.position = playerPosition + new Vector3(-1.2f, 0.2f, 0f);

        // Apply custom scale
        transform.localScale = new Vector3(scale, scale, scale);

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