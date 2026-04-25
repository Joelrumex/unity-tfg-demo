using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private Unit _player;
    private Unit _enemy;
    private bool _isPlayerTurn = true;
    public int turnNumber = 1;          // ← add this

    public void Init(Unit player, Unit enemy)
    {
        _player = player;
        _enemy = enemy;
        _isPlayerTurn = player.speed >= enemy.speed;
        turnNumber = 1;
    }

    public Unit GetCurrentUnit() => _isPlayerTurn ? _player : _enemy;

    public Unit AdvanceTurn()
    {
        _isPlayerTurn = !_isPlayerTurn;

        // Only increment turn number when it cycles back to the player
        if (_isPlayerTurn) turnNumber++;

        return GetCurrentUnit();
    }

    public bool IsPlayerTurn() => _isPlayerTurn;
}