using UnityEngine;

public class QuitterJeu : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu...");

        // Quitte l'application
        Application.Quit();

        // Utile pour tester dans l'éditeur
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}