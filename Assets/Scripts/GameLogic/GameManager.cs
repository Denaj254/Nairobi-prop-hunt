using Fusion;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Main game manager handling game state, timing, and UI.
/// Adapted from Python prototype game loop with Photon Fusion networking.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private float roundDuration = 300f; // 5 minutes
    [SerializeField] private Text gameInfoText;
    [SerializeField] private Text timerText;
    [SerializeField] private Image suspicionBar;

    private float timeSurvived = 0f;
    private float timeRemaining;
    private bool gameActive = true;

    private PropPlayerController propPlayer;
    private HunterController hunterPlayer;
    private SuspicionSystem suspicionSystem;

    private void Start()
    {
        timeRemaining = roundDuration;

        // Find player controllers
        propPlayer = FindObjectOfType<PropPlayerController>();
        hunterPlayer = FindObjectOfType<HunterController>();

        if (propPlayer != null)
            suspicionSystem = propPlayer.GetComponent<SuspicionSystem>();
    }

    private void Update()
    {
        if (!gameActive)
            return;

        UpdateTimer();
        UpdateUI();
        CheckGameOver();
    }

    /// <summary>
    /// Updates game timer and time survived counter.
    /// </summary>
    private void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        timeSurvived += Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            gameActive = false;
            GameOver(true); // Props win
        }
    }

    /// <summary>
    /// Updates the UI with current game information.
    /// Displays disguise, suspicion, and time information.
    /// </summary>
    private void UpdateUI()
    {
        if (propPlayer != null && gameInfoText != null)
        {
            gameInfoText.text = propPlayer.GetPlayerInfo();

            if (suspicionSystem != null)
            {
                gameInfoText.text += $"\nSuspicion: {suspicionSystem.GetSuspicionPercentage():F1}%";

                // Update suspicion bar
                if (suspicionBar != null)
                {
                    suspicionBar.fillAmount = suspicionSystem.GetSuspicionPercentage() / 100f;
                }
            }
        }

        // Update timer display
        if (timerText != null)
        {
            int minutes = (int)(timeRemaining / 60);
            int seconds = (int)(timeRemaining % 60);
            timerText.text = $"Time: {minutes:D2}:{seconds:D2}";
        }
    }

    /// <summary>
    /// Checks if the game should end (prop caught or time expired).
    /// </summary>
    private void CheckGameOver()
    {
        if (suspicionSystem != null && suspicionSystem.IsCaught)
        {
            gameActive = false;
            GameOver(false); // Hunters win
        }
    }

    /// <summary>
    /// Handles game over state.
    /// </summary>
    private void GameOver(bool propsWin)
    {
        gameActive = false;

        if (gameInfoText != null)
        {
            if (propsWin)
            {
                gameInfoText.text = "PROPS WIN! YOU SURVIVED!";
            }
            else
            {
                gameInfoText.text = "YOU WERE CAUGHT! HUNTERS WIN!";
            }
        }

        Debug.Log(propsWin ? "Props survived the round!" : "Prop was caught!");

        // Could trigger scene reload or next round here
        Invoke("RestartGame", 3f);
    }

    /// <summary>
    /// Restarts the game round.
    /// </summary>
    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public float GetTimeSurvived()
    {
        return timeSurvived;
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}
