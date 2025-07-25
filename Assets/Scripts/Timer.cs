using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Timer Settings")]
    public float timerDuration = 45f;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public Slider timerSlider;

    [Header("Visual Settings")]
    public Color normalColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;
    public float warningThreshold = 15f; // Secondes
    public float dangerThreshold = 5f;   // Secondes

    private float currentTime;
    private bool isTimerRunning = false;
    private Image sliderFill;

    void Start()
    {
        InitializeTimer();
        StartTimer();
    }

    private void InitializeTimer()
    {
        // Récupérer le composant Fill du slider
        if (timerSlider != null)
        {
            timerSlider.maxValue = timerDuration;
            timerSlider.value = timerDuration;
            timerSlider.interactable = false;

            // Récupérer l'image de fill pour changer la couleur
            sliderFill = timerSlider.fillRect.GetComponent<Image>();
            if (sliderFill != null)
            {
                sliderFill.color = normalColor;
            }
        }
    }

    public void StartTimer()
    {
        currentTime = timerDuration;
        isTimerRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (currentTime > 0 && isTimerRunning)
        {
            UpdateTimerDisplay();
            UpdateSliderColor(); // Changer la couleur selon le temps restant

            yield return null;
            currentTime -= Time.deltaTime;
        }

        currentTime = 0;
        UpdateTimerDisplay();
        OnTimerFinished();
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

    private void UpdateSliderColor()
    {
        if (sliderFill == null) return;

        if (currentTime <= dangerThreshold)
        {
            sliderFill.color = dangerColor;
        }
        else if (currentTime <= warningThreshold)
        {
            sliderFill.color = warningColor;
        }
        else
        {
            sliderFill.color = normalColor;
        }
    }

    private void OnTimerFinished()
    {
        Debug.Log("Timer terminé !");
        isTimerRunning = false;
        GameOver();
    }

    private void GameOver()
    {
        Debug.Log("Temps écoulé - Fin du jeu !");
        // Votre logique de fin de jeu ici
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        if (currentTime > 0)
        {
            isTimerRunning = true;
            StartCoroutine(TimerCoroutine());
        }
    }

    public void ResetTimer()
    {
        isTimerRunning = false;
        StopAllCoroutines();
        currentTime = timerDuration;
        UpdateTimerDisplay();
        if (sliderFill != null)
        {
            sliderFill.color = normalColor;
        }
    }

    public float GetRemainingTime()
    {
        return currentTime;
    }

    public float GetRemainingTimePercentage()
    {
        return currentTime / timerDuration;
    }

}
