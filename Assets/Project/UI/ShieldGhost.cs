using System.Collections;
using UnityEngine;

public class ShieldGhost : MonoBehaviour
{
    [Header("Size")]
    public float baseSize = 1f;

    [Header("Block Effect")]
    public float blockShakeDuration  = 0.3f;
    public float blockShakeIntensity = 0.08f;
    public Color blockFlashColor     = new Color(0.8f, 0.95f, 1f, 1f);

    private SpriteRenderer _sr;
    private Transform _spriteChild;
    private Vector3 _originalChildPos;
    private bool _isVisible = false;

    void Awake()
    {
        var child = new GameObject("GhostSprite");
        child.transform.SetParent(transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale    = Vector3.one;

        _sr = child.AddComponent<SpriteRenderer>();
        _sr.sortingLayerName = "Units";
        _sr.sortingOrder     = 10;
        _sr.color            = new Color(0.6f, 0.85f, 1f, 0f);

        _spriteChild        = child.transform;
        _originalChildPos   = child.transform.localPosition;
    }

    public void Appear(Sprite sprite, Vector3 playerPosition, Vector2 scale)
    {
        if (sprite == null) { Debug.LogWarning("ShieldGhost: sprite is null."); return; }
        if (scale == Vector2.zero) scale = Vector2.one;

        _sr.sprite = sprite;

        float normalizedSize = 100f / sprite.pixelsPerUnit;
        float finalX = normalizedSize * baseSize * scale.x;
        float finalY = normalizedSize * baseSize * scale.y;

        _spriteChild.localScale = new Vector3(-finalX, finalY, 1f);

        Vector2 pivot       = sprite.pivot / sprite.rect.size;
        Vector2 pivotOffset = pivot - new Vector2(0.5f, 0.5f);
        Vector3 correction  = new Vector3(pivotOffset.x * finalX, pivotOffset.y * finalY, 0f);

        transform.position = playerPosition - correction;

        StopAllCoroutines();
        StartCoroutine(AppearAnimation());
    }

    // Called by BattleManager when slash arrives
    public void Block()
    {
        if (!_isVisible) return;
        StartCoroutine(BlockAnimation());
    }

    IEnumerator AppearAnimation()
    {
        // Fade in quickly — ghost is ready before slash arrives
        yield return StartCoroutine(Fade(0f, 0.85f, 0.1f));
        _isVisible = true;

        // Idle pulse while waiting for slash
        float elapsed = 0f;
        while (elapsed < 2f && _isVisible)
        {
            float alpha = 0.85f + Mathf.Sin(elapsed * 6f) * 0.1f;
            SetAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator BlockAnimation()
    {
        _isVisible = false;

        // 1 — Flash bright white on impact
        Color originalColor = _sr.color;
        _sr.color = blockFlashColor;
        yield return new WaitForSeconds(0.05f);
        _sr.color = originalColor;

        // 2 — Shake to show impact absorption
        float elapsed = 0f;
        while (elapsed < blockShakeDuration)
        {
            float progress = elapsed / blockShakeDuration;
            float intensity = blockShakeIntensity * (1f - progress);   // Fade shake out

            _spriteChild.localPosition = _originalChildPos + new Vector3(
                Random.Range(-intensity, intensity),
                Random.Range(-intensity, intensity),
                0f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3 — Reset position then fade out
        _spriteChild.localPosition = _originalChildPos;
        yield return StartCoroutine(Fade(0.85f, 0f, 0.3f));
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