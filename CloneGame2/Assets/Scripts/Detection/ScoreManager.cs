using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    [SerializeField] private int pointsPerBall = 20;

    [Header("UI")]
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Score Panel")]
    [SerializeField] private GameObject scorePanel;

    private int currentScore;
    private int bestScore;

    private const string BestScoreKey = "BestScore";

    private void Awake()
    {
        Instance = this;

        bestScore =
            PlayerPrefs.GetInt(
                BestScoreKey,
                0
            );

        currentScore = 0;

        UpdateUI();

        if (scorePanel != null)
        {
            scorePanel.SetActive(false);
        }
    }

    public void AddScore(int numberOfBalls)
    {
        int pointsEarned =
            numberOfBalls *
            pointsPerBall;

        currentScore += pointsEarned;

        if (currentScore > bestScore)
        {
            bestScore = currentScore;

            PlayerPrefs.SetInt(
                BestScoreKey,
                bestScore
            );

            PlayerPrefs.Save();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text =
                currentScore.ToString();
        }

        if (bestScoreText != null)
        {
            bestScoreText.text =
                bestScore.ToString();
        }
    }

    public void ShowScorePanel()
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(true);
        }
    }

    public void HideScorePanel()
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(false);
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetBestScore()
    {
        return bestScore;
    }

    public void ResetScore()
    {
        currentScore = 0;

        UpdateUI();
    }

    public void ResetBestScore()
    {
        bestScore = 0;

        PlayerPrefs.DeleteKey(
            BestScoreKey
        );

        PlayerPrefs.Save();

        UpdateUI();
    }
}