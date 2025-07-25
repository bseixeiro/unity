using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CoinGameManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timerDuration = 45f;

    [Header("Coin Settings")]
    [SerializeField] private int totalCoinsToCollect = 10;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private GameObject defeatScreen; // Panel pour l'écran de défaite
    [SerializeField] private TextMeshProUGUI defeatMessage;
    [SerializeField] private GameObject victoryScreen; // Panel pour l'écran de victoire
    [SerializeField] private TextMeshProUGUI victoryMessage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton; // Bouton pour retourner au menu Home
    [SerializeField] private GameObject gameUI; // Panel contenant le timer et compteur de pièces

    // Variables privées
    private ThirdPersonCamera camera;
    private float currentTime;
    private bool isTimerRunning = false;
    private bool gameEnded = false;
    private int coinsCollected = 0;
    private int coinsRemaining;
    private float completionTime; // Temps mis pour finir le niveau

    void Start()
    {
        InitializeGame();

        camera = FindObjectOfType<ThirdPersonCamera>();
    }

    private void InitializeGame()
    {
        // Initialiser les variables
        currentTime = timerDuration;
        coinsCollected = 0;
        coinsRemaining = totalCoinsToCollect;
        gameEnded = false;
        isTimerRunning = true;
        completionTime = 0f;

        // Initialiser l'UI
        InitializeUI();

        // Démarrer le timer
        StartCoroutine(TimerCoroutine());

        // Compter automatiquement les pièces dans la scène
        CountCoinsInScene();
    }

    private void InitializeUI()
    {
        // Timer
        if (timerSlider != null)
        {
            timerSlider.maxValue = timerDuration;
            timerSlider.value = timerDuration;
            timerSlider.interactable = false;
        }

        // Affichage des pièces
        UpdateCoinsDisplay();

        // Écran de défaite
        if (defeatScreen != null)
            defeatScreen.SetActive(false);

        // Écran de victoire
        if (victoryScreen != null)
            victoryScreen.SetActive(false);

        // UI du jeu
        if (gameUI != null)
            gameUI.SetActive(true);

        // Boutons
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (homeButton != null)
            homeButton.onClick.AddListener(GoToHome);
    }

    private void CountCoinsInScene()
    {
        // Compter automatiquement les pièces avec le tag "Coin"
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        if (coins.Length > 0)
        {
            totalCoinsToCollect = coins.Length;
            coinsRemaining = totalCoinsToCollect;
            UpdateCoinsDisplay();
        }
    }

    private int CountCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        return coins.Length;
    }

    private IEnumerator TimerCoroutine()
    {
        while (currentTime > 0 && isTimerRunning && !gameEnded)
        {
            UpdateTimerDisplay();
            yield return null;
            currentTime -= Time.deltaTime;
        }

        // Timer terminé
        if (!gameEnded)
        {
            currentTime = 0;
            UpdateTimerDisplay();
            CheckGameEnd();
        }
    }

    private void UpdateTimerDisplay()
    {
        // Mettre à jour le texte
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Mettre à jour le slider
        if (timerSlider != null)
        {
            timerSlider.value = currentTime;
        }
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsText != null)
        {
            coinsText.text = $"Pièces: {coinsCollected}/{totalCoinsToCollect}";
            // Alternative: coinsText.text = $"Pièces restantes: {coinsRemaining}";
        }
    }

    // Fonction à appeler quand une pièce est collectée
    public void CollectCoin()
    {
        if (gameEnded) return;

        coinsCollected = totalCoinsToCollect - CountCoins() +1;
        coinsRemaining = totalCoinsToCollect - coinsCollected;

        // Mettre à jour l'affichage
        UpdateCoinsDisplay();

        // Vérifier si le jeu est terminé
        CheckGameEnd();
    }

    private void CheckGameEnd()
    {
        if (gameEnded) return;

        if (coinsRemaining <= 0)
        {
            // Victoire !
            Victory();
        }
        else if (currentTime <= 0)
        {
            // Défaite - temps écoulé
            Defeat();
        }
    }

    private void Victory()
    {
        gameEnded = true;
        camera.SetCursorLock(false);
        isTimerRunning = false;
        completionTime = timerDuration - currentTime; // Calculer le temps de completion

        Debug.Log("Victoire ! Toutes les pièces collectées !");

        ShowVictoryScreen();
    }

    private void Defeat()
    {
        gameEnded = true;
        camera.SetCursorLock(false);
        isTimerRunning = false;

        Debug.Log("Défaite ! Temps écoulé !");

        ShowDefeatScreen();
    }

    private void ShowVictoryScreen()
    {
        // Masquer l'UI du jeu
        if (gameUI != null)
            gameUI.SetActive(false);

        // Afficher l'écran de victoire
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);

            // Message personnalisé simple
            if (victoryMessage != null)
            {
                int minutes = Mathf.FloorToInt(completionTime / 60);
                int seconds = Mathf.FloorToInt(completionTime % 60);

                victoryMessage.text = $"Félicitations !\n\n" +
                                    $"Vous avez collecté toutes les pièces !\n" +
                                    $"Temps: {minutes:00}:{seconds:00}";
            }
        }

        // Optionnel: mettre le jeu en pause
        // Time.timeScale = 0;
    }

    private void ShowDefeatScreen()
    {
        // Masquer l'UI du jeu
        if (gameUI != null)
            gameUI.SetActive(false);

        // Afficher l'écran de défaite
        if (defeatScreen != null)
        {
            defeatScreen.SetActive(true);

            // Message personnalisé
            if (defeatMessage != null)
            {
                defeatMessage.text = $"Temps écoulé !\nPièces collectées: {coinsCollected}/{totalCoinsToCollect}\nIl vous restait {coinsRemaining} pièces !";
            }
        }

        // Optionnel: mettre le jeu en pause
        // Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Remettre le temps à la normale si il était en pause
        Time.timeScale = 1;

        // Recharger la scène actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToHome()
    {
        // Remettre le temps à la normale si il était en pause
        Time.timeScale = 1;

        // Charger la scène Home
        SceneManager.LoadScene("Home");
    }

    // Fonctions utilitaires publiques
    public int GetCoinsRemaining()
    {
        return coinsRemaining;
    }

    public int GetCoinsCollected()
    {
        return coinsCollected;
    }

    public float GetRemainingTime()
    {
        return currentTime;
    }

    public float GetCompletionTime()
    {
        return completionTime;
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }

    // Fonction pour ajouter du temps bonus (power-up par exemple)
    public void AddBonusTime(float seconds)
    {
        if (!gameEnded)
        {
            currentTime += seconds;
            currentTime = Mathf.Min(currentTime, timerDuration); // Ne pas dépasser le temps max
        }
    }
}