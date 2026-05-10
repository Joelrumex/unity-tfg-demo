using System.Collections;
using UnityEngine;

public class CheckpointStatue : MonoBehaviour
{
    [Header("Heal Settings")]
    public float healRadius = 2f;
    public float healDelay = 0.5f;
    public bool healOnce = false;

    [Header("Shader")]
    public SpriteRenderer statueRenderer;
    public string colorPropertyName = "_GlowColor";
    public Color glowColor = Color.white;
    public float minIntensity = 0f;
    public float maxIntensity = 2f;

    public ParticleSystem healParticles;
    private float _currentIntensity = 0f;
    public float glowSpeed = 5f; 
    private bool _hasHealed = false;
    private bool _isHealing = false;
    private PlayerHealth _player;
    private Transform _playerTransform;
    private Material _material;

    void Start()
    {
        if (statueRenderer != null)
        {
            _material = statueRenderer.material;
            SetGlow(minIntensity);
        }
    }

    void Update()
    {
        // Only drive the glow — no heal logic here
        UpdateGlowByDistance();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        _player = col.GetComponent<PlayerHealth>();
        _playerTransform = col.transform;

        // Set glow immediately so there's no zero-frame flicker
        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        float normalized = 1f - Mathf.Clamp01(distance / healRadius);
        SetGlow(Mathf.Lerp(minIntensity, maxIntensity, normalized));

        if (!_isHealing && (!healOnce || !_hasHealed))
            StartCoroutine(HealPlayer());
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        _playerTransform = null;

        // Cancel pending heal if player left before it triggered
        if (_isHealing && !_hasHealed)
        {
            StopCoroutine(nameof(HealPlayer));
            _isHealing = false;
        }

        if (_material != null) SetGlow(minIntensity);
    }



    void UpdateGlowByDistance()
    {
        if (_material == null) return;

        float targetIntensity = 0f;

        if (_playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, _playerTransform.position);
            float normalized = 1f - Mathf.Clamp01(distance / healRadius);
            targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, normalized);
        }

        // Smoothly move toward target instead of snapping
        _currentIntensity = Mathf.Lerp(_currentIntensity, targetIntensity, Time.deltaTime * glowSpeed);
        SetGlow(_currentIntensity);
    }

    IEnumerator HealPlayer()
    {
        _isHealing = true;

        yield return new WaitForSeconds(healDelay);

        if (_player == null)
        {
            _isHealing = false;
            yield break;
        }

        _player.RestoreFullHP();
        _hasHealed = true;

        if (healParticles != null) healParticles.Play();

        yield return StartCoroutine(HealFlash());

        _isHealing = false;
    }

    IEnumerator HealFlash()
    {
        for (int i = 0; i < 4; i++)
        {
            SetGlow(maxIntensity);
            yield return new WaitForSeconds(0.1f);
            SetGlow(minIntensity);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void SetGlow(float intensity)
    {
        _material.SetColor(colorPropertyName, glowColor * intensity);
    }

    void OnDestroy()
    {
        if (_material != null) Destroy(_material);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
        Gizmos.DrawSphere(transform.position, healRadius);
    }
}