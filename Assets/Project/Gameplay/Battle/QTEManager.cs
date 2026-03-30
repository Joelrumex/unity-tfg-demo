using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QTEResult { Perfect, Good, Failed }

public class QTEManager : MonoBehaviour
{
    [Header("Settings")]
    public int keyCount = 4;               // How many keys in the sequence
    public float timePerKey = 1.2f;        // Seconds allowed per key

    // Events — BattleManager listens to these
    public System.Action<QTEResult> OnQTEComplete;

    private static readonly KeyCode[] _pool = {
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
    };

    private List<KeyCode> _sequence = new List<KeyCode>();
    private int _currentIndex = 0;
    private int _successCount = 0;
    private bool _isRunning = false;

    private QTEUI _qteUI;   // We'll make this next

    void Awake()
    {
        _qteUI = GetComponent<QTEUI>();
    }

    public void StartQTE()
    {
        _sequence.Clear();
        _currentIndex = 0;
        _successCount = 0;
        _isRunning = true;

        // Build a random sequence from W A S D
        for (int i = 0; i < keyCount; i++)
            _sequence.Add(_pool[Random.Range(0, _pool.Length)]);

        _qteUI.ShowQTE(_sequence);
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        for (int i = 0; i < _sequence.Count; i++)
        {
            _currentIndex = i;
            _qteUI.HighlightKey(i);         // Light up the current key prompt

            float elapsed = 0f;
            bool pressed = false;

            while (elapsed < timePerKey)
            {
                elapsed += Time.deltaTime;
                _qteUI.UpdateTimerBar(i, 1f - (elapsed / timePerKey));

                if (Input.GetKeyDown(_sequence[i]))
                {
                    _successCount++;
                    _qteUI.KeySuccess(i);
                    pressed = true;
                    break;
                }

                // Wrong key pressed — count as fail
                if (AnyWrongKeyPressed(_sequence[i]))
                {
                    _qteUI.KeyFail(i);
                    pressed = true;   // move on, don't wait for timeout
                    break;
                }

                yield return null;
            }

            if (!pressed)
                _qteUI.KeyFail(i);      // Timed out

            yield return new WaitForSeconds(0.15f);   // Brief pause between keys
        }

        _isRunning = false;
        FinishQTE();
    }

    bool AnyWrongKeyPressed(KeyCode correct)
    {
        foreach (var key in _pool)
            if (key != correct && Input.GetKeyDown(key))
                return true;
        return false;
    }

    void FinishQTE()
    {
        QTEResult result;

        if (_successCount == _sequence.Count)
            result = QTEResult.Perfect;      // All keys hit
        else if (_successCount >= _sequence.Count / 2)
            result = QTEResult.Good;         // At least half hit
        else
            result = QTEResult.Failed;       // Mostly missed

        _qteUI.ShowQTEResult(result);
        StartCoroutine(DelayedCallback(result));
    }

    IEnumerator DelayedCallback(QTEResult result)
    {
        yield return new WaitForSeconds(1f);
        _qteUI.HideQTE();
        OnQTEComplete?.Invoke(result);
    }
}