using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS }
public class GameManager : MonoBehaviour
{

    public GameState currentGameState = GameState.GS_GAME;
    public static GameManager instance;
    public Canvas inGameCanvas;
    public Canvas pauseMenuCanvas;
    public Canvas levelCompletedCanvas;
    public Canvas optionsCanvas;
    public Slider volumeSlider;



    public GameObject EndingScreen;
    
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text enemyKillsText;
    public TMP_Text HighScore193363_192594Text;
    public TMP_Text afterLevelScoreText;
    public TMP_Text qualityLevelText;


    private int score = 0;
    private int keysFound = 0;
    private int lifePoints = 3;
    private int enemyKills = 0;
    
    public Image[] keysTab;
    public Image[] hearthsTab;


    const string keyHighScore193363_192594 = "HighScore193363_192594Level1";

    public float timer = 0;
    public float minutes = 0;
    public float seconds = 0;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentGameState== GameState.GS_PAUSEMENU)
            {
                InGame();
            }
            else if(currentGameState== GameState.GS_GAME)
            {
                PauseMenu();
            }
        }

        if (currentGameState == GameState.GS_GAME)
        {
            timer += Time.deltaTime;
            minutes = Mathf.FloorToInt(timer / 60);
            seconds = Mathf.FloorToInt(timer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(keyHighScore193363_192594))
        {
            PlayerPrefs.SetInt(keyHighScore193363_192594, 0);
        }
  
        InGame();
        instance = this;
        scoreText.text = score.ToString();
        enemyKillsText.text = enemyKills.ToString();
        EndingScreen.SetActive(false);
        for (int i=0;i<keysTab.Length;i++)
        {
            keysTab[i].color = Color.grey;
        }

        for(int i = lifePoints; i < hearthsTab.Length; i++)
        {
            hearthsTab[i].gameObject.SetActive(false);
        }
    }

    void SetGameState( GameState newGameState )
    {
        currentGameState = newGameState;
        Time.timeScale = 1;

        if (currentGameState == GameState.GS_GAME)
        {
            inGameCanvas.enabled = true;
            pauseMenuCanvas.enabled = false;
            levelCompletedCanvas.enabled = false;
            optionsCanvas.enabled = false;
        }
        else if(currentGameState == GameState.GS_PAUSEMENU)
        {
            pauseMenuCanvas.enabled = true;
            inGameCanvas.enabled = false;
            levelCompletedCanvas.enabled = false;
            optionsCanvas.enabled = false;

        }
        else if(currentGameState == GameState.GS_OPTIONS)
        {
            pauseMenuCanvas.enabled = false;
            inGameCanvas.enabled = false;
            levelCompletedCanvas.enabled = false;
            optionsCanvas.enabled = true;
            qualityLevelText.text = QualitySettings.names[QualitySettings.GetQualityLevel()].ToString();


            Time.timeScale = 0;
        }
        else if(currentGameState == GameState.GS_LEVELCOMPLETED)
        {
            Scene currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "193363,192594")
            {
                int HighScore193363_192594 = PlayerPrefs.GetInt(keyHighScore193363_192594);
                if(HighScore193363_192594 < score)
                {
                    HighScore193363_192594 = score;
                    PlayerPrefs.SetInt(keyHighScore193363_192594, HighScore193363_192594);

                }

                afterLevelScoreText.text = "Your score: " + score.ToString();
                HighScore193363_192594Text.text = "High score: " + HighScore193363_192594.ToString();
            }



               levelCompletedCanvas.enabled = true;
            inGameCanvas.enabled = false;
            pauseMenuCanvas.enabled = false;
            optionsCanvas.enabled = false;
        }
        else
        {
            inGameCanvas.enabled = false;
            pauseMenuCanvas.enabled = false;
            levelCompletedCanvas.enabled = false;
            optionsCanvas.enabled = false;

        }
    }
    void PauseMenu()
    {
        SetGameState(GameState.GS_PAUSEMENU);
    }
    void InGame()
    {
        SetGameState(GameState.GS_GAME);
    }
    public void LevelCompleted()
    {
        SetGameState(GameState.GS_LEVELCOMPLETED);
    }
    void GameOver()
    {
        SetGameState(GameState.GS_GAME_OVER);
    }

    public void Options()
    {
        SetGameState(GameState.GS_OPTIONS);
    }

    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void AddKeys(Color color) 
    {
        if ((keysFound - 1) < keysTab.Length)
        {
            keysFound += 1;
            keysTab[keysFound-1].color = color;
        }
    }

    public void AddLife()
    {
        hearthsTab[lifePoints].gameObject.SetActive(true);
        lifePoints += 1;
    }

    public void RemoveLife()
    {
        lifePoints -= 1;
        hearthsTab[lifePoints].gameObject.SetActive(false);
    }

    public void AddEnemyKill()
    {
        enemyKills += 1;
        enemyKillsText.text = enemyKills.ToString();
    }

    public void EnableEndingScreen()
    {
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath("MainMenu");
        if (sceneIndex >= 0)
        {
            SceneManager.LoadSceneAsync(sceneIndex); //³adowanie sceny ³¹cz¹cej gry
        }
        else
        {
            //sceneIndex jest równe -1. Nie znaleziono sceny.
            //³adowanie innej sceny docelowo na laboratorium
        }
    }

    public void OnResumeButtonClicked()
    {
        InGame();
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnReturnToMainMenuButtonClicked()
    {
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath("MainMenu");
        if (sceneIndex >= 0)
        {
            SceneManager.LoadSceneAsync(sceneIndex); //³adowanie sceny ³¹cz¹cej gry
        }
        else
        {
            //sceneIndex jest równe -1. Nie znaleziono sceny.
            //³adowanie innej sceny docelowo na laboratorium
        }
    }

    public void OnOptionsButtonClicked()
    {
        Options();
    }

    public void OnDecreaseQualityButton()
    {
        QualitySettings.DecreaseLevel();
        qualityLevelText.text = QualitySettings.names[QualitySettings.GetQualityLevel()].ToString();
    }

    public void OnIncreaseQualityButton()
    {
        QualitySettings.IncreaseLevel();
        qualityLevelText.text = QualitySettings.names[QualitySettings.GetQualityLevel()].ToString();
    }

    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
