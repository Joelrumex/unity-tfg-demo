using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Nombre de la escena a cargar al pulsar Start
    [SerializeField] private string gameSceneName = "ForestPlatformerScene";

    /// <summary>
    /// Carga la escena principal del juego.
    /// Asigna este método al botón "Start" en el Inspector.
    /// </summary>
    public void OnStartButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Cierra la aplicación.
    /// Asigna este método al botón "Quit" en el Inspector.
    /// En el editor de Unity esto no cierra la ventana, solo funciona en build.
    /// </summary>
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
