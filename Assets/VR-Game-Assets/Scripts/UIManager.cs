using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TMP_Text timerText;
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public Image progressBarFill;
    public WaveManager waveManager;

    public int pointsPerCube = 10;
    public int penaltyPerWrongCube = 5;

    private int score = 0;
    private int bestScore;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waveManager == null)
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }

        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = bestScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        int totalSeconds = Mathf.CeilToInt(waveManager.timeLeft);
        timerText.text = (totalSeconds / 60) + ":" + (totalSeconds % 60).ToString("00");
        progressBarFill.fillAmount = waveManager.timeLeft / waveManager.waveDuration;
        scoreText.text = score.ToString();
    }

    public void AddScore()
    {
        if (!waveManager.gameOver)
        {
            score += pointsPerCube;
        }
    }

    public void RemoveScore()
    {
        if (!waveManager.gameOver)
        {
            score = Mathf.Max(0, score - penaltyPerWrongCube);
        }
    }

    public void EndGame()
    {
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        bestScoreText.text = bestScore.ToString();
    }
}
