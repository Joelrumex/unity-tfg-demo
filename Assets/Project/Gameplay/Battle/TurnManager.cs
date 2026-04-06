using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private Unit _player;
    private Unit _enemy;
    private bool _isPlayerTurn = true;

    public void Init(Unit player, Unit enemy)
    {
        _player = player;
        _enemy = enemy;
        // Whoever has higher speed goes first
        _isPlayerTurn = player.speed >= enemy.speed;
    }

    public Unit GetCurrentUnit()
    {
        if (_player == null || _enemy == null)
        {
            Debug.LogError("TurnManager: Player or Enemy is null!");
            return _player ?? _enemy;
        }
        return _isPlayerTurn ? _player : _enemy;
    }

    public Unit AdvanceTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;
        return GetCurrentUnit();
    }

    public bool IsPlayerTurn() => _isPlayerTurn;
}