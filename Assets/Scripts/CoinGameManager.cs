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
    [SerializeField] private GameObject defeatScreen; // Panel pour l'�cran de d�faite
    [SerializeField] private TextMeshProUGUI defeatMessage;
    [SerializeField] private GameObject victoryScreen; // Panel pour l'�cran de victoire
    [SerializeField] private TextMeshProUGUI victoryMessage;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton; // Bouton pour retourner au menu Home
    [SerializeField] private GameObject gameUI; // Panel contenant le timer et compteur de pi�ces

    // Variables priv�es
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

        // D�marrer le timer
        StartCoroutine(TimerCoroutine());

        // Compter automatiquement les pi�ces dans la sc�ne
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

        // Affichage des pi�ces
        UpdateCoinsDisplay();

        // �cran de d�faite
        if (defeatScreen != null)
            defeatScreen.SetActive(false);

        // �cran de victoire
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
        // Compter automatiquement les pi�ces avec le tag "Coin"
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

        // Timer termin�
        if (!gameEnded)
        {
            currentTime = 0;
            UpdateTimerDisplay();
            CheckGameEnd();
        }
    }

    private void UpdateTimerDisplay()
    {
        // Mettre � jour le texte
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Mettre � jour le slider
        if (timerSlider != null)
        {
            timerSlider.value = currentTime;
        }
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsText != null)
        {
            coinsText.text = $"Pi�ces: {coinsCollected}/{totalCoinsToCollect}";
            // Alternative: coinsText.text = $"Pi�ces restantes: {coinsRemaining}";
        }
    }

    // Fonction � appeler quand une pi�ce est collect�e
    public void CollectCoin()
    {
        if (gameEnded) return;

        coinsCollected = totalCoinsToCollect - CountCoins() +1;
        coinsRemaining = totalCoinsToCollect - coinsCollected;

        // Mettre � jour l'affichage
        UpdateCoinsDisplay();

        // V�rifier si le jeu est termin�
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
            // D�faite - temps �coul�
            Defeat();
        }
    }

    private void Victory()
    {
        gameEnded = true;
        camera.SetCursorLock(false);
        isTimerRunning = false;
        completionTime = timerDuration - currentTime; // Calculer le temps de completion

        Debug.Log("Victoire ! Toutes les pi�ces collect�es !");

        ShowVictoryScreen();
    }

    private void Defeat()
    {
        gameEnded = true;
        camera.SetCursorLock(false);
        isTimerRunning = false;

        Debug.Log("D�faite ! Temps �coul� !");

        ShowDefeatScreen();
    }

    private void ShowVictoryScreen()
    {
        // Masquer l'UI du jeu
        if (gameUI != null)
            gameUI.SetActive(false);

        // Afficher l'�cran de victoire
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);

            // Message personnalis� simple
            if (victoryMessage != null)
            {
                int minutes = Mathf.FloorToInt(completionTime / 60);
                int seconds = Mathf.FloorToInt(completionTime % 60);

                victoryMessage.text = $"F�licitations !\n\n" +
                                    $"Vous avez collect� toutes les pi�ces !\n" +
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

        // Afficher l'�cran de d�faite
        if (defeatScreen != null)
        {
            defeatScreen.SetActive(true);

            // Message personnalis�
            if (defeatMessage != null)
            {
                defeatMessage.text = $"Temps �coul� !\nPi�ces collect�es: {coinsCollected}/{totalCoinsToCollect}\nIl vous restait {coinsRemaining} pi�ces !";
            }
        }

        // Optionnel: mettre le jeu en pause
        // Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Remettre le temps � la normale si il �tait en pause
        Time.timeScale = 1;

        // Recharger la sc�ne actuelle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToHome()
    {
        // Remettre le temps � la normale si il �tait en pause
        Time.timeScale = 1;

        // Charger la sc�ne Home
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
            currentTime = Mathf.Min(currentTime, timerDuration); // Ne pas d�passer le temps max
        }
    }
}