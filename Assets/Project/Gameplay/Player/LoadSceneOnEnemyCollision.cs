using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnEnemyCollision : MonoBehaviour
{
    [SerializeField] private string battleSceneName = "BattleScene";

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(battleSceneName);
        }
    }
}