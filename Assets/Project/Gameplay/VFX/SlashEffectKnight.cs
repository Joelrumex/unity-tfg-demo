using System.Collections;
using UnityEngine;

public class SlashEffectKnight : MonoBehaviour
{
    [Header("Sprite")]
    public Sprite slashSprite;          // Drag your slash sprite here in Inspector
    public Color slashColor = Color.white;

    [Header("Settings")]
    public int slashCount   = 3;        // How many slashes per hit
    public float duration   = 0.2f;     // How long each slash lasts
    public Vector2 baseScale = new Vector2(1.5f, 1.5f);   // Base size
    public float scaleVariance = 0.3f;  // Random size variation per slash
    public float angleVariance = 30f;   // Random rotation variance per slash

    void Awake()
    {
        // Pre-create one SpriteRenderer child per slash
        for (int i = 0; i < slashCount; i++)
        {
            var go = new GameObject($"Slash_{i}");
            go.transform.SetParent(transform);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = slashSprite;
            sr.color            = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
            sr.sortingLayerName = "Units";
            sr.sortingOrder     = 5;
        }
    }

    public void Play(Vector3 targetPosition)
    {
        StopAllCoroutines();
        StartCoroutine(PlaySequence(targetPosition));
    }

    IEnumerator PlaySequence(Vector3 center)
    {
        for (int i = 0; i < slashCount; i++)
        {
            var child = transform.GetChild(i);
            StartCoroutine(AnimateSlash(child, center, i));
            yield return new WaitForSeconds(0.07f);   // Stagger between slashes
        }
    }

    IEnumerator AnimateSlash(Transform slashTransform, Vector3 center, int index)
    {
        var sr = slashTransform.GetComponent<SpriteRenderer>();

        // Randomize position slightly so slashes don't stack
        Vector3 offset = new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(-0.3f, 0.3f),
            0f
        );
        slashTransform.position = center + offset;

        // Randomize rotation — base diagonal angle + variance
        float angle = -45f + (index * 20f) + Random.Range(-angleVariance, angleVariance);
        slashTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Randomize scale slightly
        float scaleMultiplier = 1f + Random.Range(-scaleVariance, scaleVariance);
        slashTransform.localScale = new Vector3(
            baseScale.x * scaleMultiplier,
            baseScale.y * scaleMultiplier,
            1f
        );

        // Animate — quick fade in then fade out
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Fade in first 30%, hold, fade out last 40%
            float alpha = t < 0.3f
                ? Mathf.Lerp(0f, 1f, t / 0.3f)
                : t > 0.6f
                    ? Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f)
                    : 1f;

            // Slight scale punch — grows then shrinks back
            float scalePunch = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;
            slashTransform.localScale = new Vector3(
                baseScale.x * scaleMultiplier * scalePunch,
                baseScale.y * scaleMultiplier * scalePunch,
                1f
            );

            sr.color = new Color(slashColor.r, slashColor.g, slashColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset
        sr.color = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
        slashTransform.localScale = Vector3.one;
    }
}