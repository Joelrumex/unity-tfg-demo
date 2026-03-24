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
        return _isPlayerTurn ? _player : _enemy;
    }

    public Unit AdvanceTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;
        return GetCurrentUnit();
    }

    public bool IsPlayerTurn() => _isPlayerTurn;
}