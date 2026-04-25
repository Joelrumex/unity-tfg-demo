using System.Collections;
using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    [Header("Slash Settings")]
    public int slashCount = 3;              // How many slash lines per hit
    public float slashDuration = 0.18f;     // How long each slash lasts
    public float slashLength = 1.8f;        // Length of each slash line
    public Color slashColor = new Color(1f, 1f, 1f, 1f);
    public float lineWidth = 0.06f;

    private LineRenderer[] _lines;

    void Awake()
    {
        // Pre-create all line renderers so we don't allocate during gameplay
        _lines = new LineRenderer[slashCount];
        for (int i = 0; i < slashCount; i++)
        {
            var go = new GameObject($"SlashLine_{i}");
            go.transform.SetParent(transform);

            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth    = lineWidth;
            lr.endWidth      = lineWidth * 0.3f;   // Taper toward the end
            lr.material      = CreateSlashMaterial();
            lr.startColor    = slashColor;
            lr.endColor      = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);
            lr.sortingLayerName = "Units";
            lr.sortingOrder     = 5;
            lr.enabled          = false;

            _lines[i] = lr;
        }
    }

    // Creates an unlit material so the slash is always bright
    Material CreateSlashMaterial()
    {
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = Color.white;
        return mat;
    }

    // Call this from BattleManager when enemy attacks
    public void Play(Vector3 targetPosition)
    {
        StopAllCoroutines();
        StartCoroutine(PlaySequence(targetPosition));
    }

    IEnumerator PlaySequence(Vector3 center)
    {
        // Play slashes one after another with a tiny gap
        for (int i = 0; i < slashCount; i++)
        {
            StartCoroutine(AnimateSingleSlash(_lines[i], center, i));
            yield return new WaitForSeconds(0.06f);   // Stagger each slash
        }
    }

    IEnumerator AnimateSingleSlash(LineRenderer lr, Vector3 center, int index)
    {
        lr.enabled = true;

        // Each slash has a slightly different angle for a natural feel
        float baseAngle  = Random.Range(-50f, -20f);           // Diagonal downward
        float angleOffset = index * 15f;                        // Spread slashes apart
        float angle = (baseAngle + angleOffset) * Mathf.Deg2Rad;

        // Direction vector for this slash
        Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

        // Randomize start point slightly so they don't all overlap perfectly
        Vector3 startOffset = new Vector3(
            Random.Range(-0.3f, 0.1f),
            Random.Range(-0.2f, 0.3f),
            0f
        );

        Vector3 from = center + startOffset;
        Vector3 to   = from + dir * slashLength;

        // Animate the slash scaling from zero to full length then fading
        float elapsed = 0f;
        while (elapsed < slashDuration)
        {
            float t     = elapsed / slashDuration;
            float scale = t < 0.4f
                ? Mathf.Lerp(0f, 1f, t / 0.4f)    // Grow in first 40%
                : 1f;                               // Stay full for rest

            float alpha = t < 0.5f
                ? 1f
                : Mathf.Lerp(1f, 0f, (t - 0.5f) / 0.5f);  // Fade out last 50%

            lr.SetPosition(0, from);
            lr.SetPosition(1, Vector3.Lerp(from, to, scale));

            lr.startColor = new Color(slashColor.r, slashColor.g, slashColor.b, alpha);
            lr.endColor   = new Color(slashColor.r, slashColor.g, slashColor.b, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        lr.enabled = false;
    }
}