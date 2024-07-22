using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive = false;

    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingMenuMoneyText, gameOverMoneyText, finishGameText;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;

    public AudioSource gameMusicAudoSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;

    public DailyReward dailyReward;

    int currentLevel;
    int score;


    // Start is called before the first frame update
    void Start()
    {
        Current = this;
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        if (SceneManager.GetActiveScene().name != "Level " + currentLevel)
        {
            SceneManager.LoadScene("Level " + currentLevel);
        }
        else
        {
            dailyReward.InitializeDailyReward();
            currentLevelText.text = (currentLevel + 1).ToString();
            nextLevelText.text = (currentLevel + 2).ToString();
            UpdateMoneyTexts();
        }
        gameMusicAudoSource = Camera.main.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            PlayerController player = PlayerController.Current;
            float distance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);
        }
    }
    public void StartLevel()
    {
        maxDistance = finishLine.transform.position.z - PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.Current.animator.SetBool("running", true);
        gameActive = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene("level " + currentLevel + 1);
    }
    public void GameOver()
    {
        UpdateMoneyTexts();
        gameMusicAudoSource.Stop();
        gameMusicAudoSource.PlayOneShot(gameOverAudioClip);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;

    }

    public void FinishGame()
    {
        GiveMoneyToPlayer(score);
        gameMusicAudoSource.Stop();
        gameMusicAudoSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);

        finishScoreText.text = score.ToString();

        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;


    }
    public void ChangeScore(int increment)
    {
        score += increment;
        scoreText.text = score.ToString();
    }
    public void UpdateMoneyTexts()
    {
        int money = PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text = money.ToString();
        gameOverMoneyText.text = money.ToString();
        finishGameText.text = money.ToString();
    }

    public void GiveMoneyToPlayer(int increment)
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + increment);
        PlayerPrefs.SetInt("money", money + score);
        UpdateMoneyTexts();

    }
}
