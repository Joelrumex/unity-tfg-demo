using System.Collections;
using UnityEngine;

public class ShieldGhost : MonoBehaviour
{
    [Header("Settings")]
    public float scale = 1.0f;

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = new Color(1f, 1f, 1f, 0f);   // Start invisible
    }

    public void Appear(Sprite sprite, Vector3 playerPosition)
    {
        if (sprite == null) return;

        _sr.sprite = sprite;

        // Position ghost between enemy and player, slightly in front
        transform.position = playerPosition + new Vector3(-1.2f, 0.2f, 0f);

        // Apply custom scale
        transform.localScale = new Vector3(scale, scale, scale);

        StartCoroutine(GhostAnimation());
    }

    IEnumerator GhostAnimation()
    {
        // 1 — Fade in quickly
        yield return StartCoroutine(Fade(0f, 0.7f, 0.15f));

        // 2 — Pulse glow (flicker alpha slightly)
        float elapsed = 0f;
        while (elapsed < 0.6f)
        {
            float alpha = 0.7f + Mathf.Sin(elapsed * 20f) * 0.15f;
            SetAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3 — Fade out slowly
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