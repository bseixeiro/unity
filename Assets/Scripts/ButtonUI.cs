using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu");

        // Quitte l'application
        Application.Quit();

        // Utile pour tester dans l'�diteur
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void StartGame()
    {
        Debug.Log("D�marrer le jeu");

        // Charge la sc�ne de jeu
        SceneManager.LoadScene("Game");
    }

    public void OpenCredits()
    {
        Debug.Log("Go to cr�dits");

        // Charge la sc�ne de jeu
        SceneManager.LoadScene("Credits");
    }
}
