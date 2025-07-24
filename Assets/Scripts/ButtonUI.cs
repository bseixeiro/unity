using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");

        // Quitte l'application
        Application.Quit();

        // Utile pour tester dans l'éditeur
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void StartGame()
    {
        Debug.Log("Démarrer le jeu");

        // Charge la scène de jeu
        SceneManager.LoadScene("Game");
    }

    public void OpenCredits()
    {
        Debug.Log("Go to crédits");

        // Charge la scène de jeu
        SceneManager.LoadScene("Credits");
    }
}
