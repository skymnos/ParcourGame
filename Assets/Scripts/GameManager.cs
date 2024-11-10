using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreTextMenu;

    public float timer;

    [SerializeField] private TextMeshProUGUI timerText;

    public Vector3 spawnPoint;

    private int levelIndex;

    [SerializeField] private PlayerController playerController;

    private bool pauseTimer;

    private bool menuOpened;

    private void Start()
    {
        menuOpened = false;

        timer = 0;
        timerText.text = Math.Round(timer, 3).ToString();
        pauseTimer = true;
        //playerController.Respawn(spawnPoint);
        Time.timeScale = 1.0f;
    }
    void Update()
    {

        if (!pauseTimer)
        {
            timer += Time.deltaTime;
            timerText.text = Math.Round(timer, 3).ToString();
        }
    }

    public void ResetLevelButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ResetLevel();
        }
    }

    public void ResetLevel()
    {
        timer = 0;
        timerText.text = Math.Round(timer, 3).ToString();
        pauseTimer = true;
        playerController.Respawn(spawnPoint);
        Time.timeScale = 1.0f;
    }

    public void LevelFinished()
    {
        PauseGame();
        scoreTextMenu.text = timerText.text;
    }

    public void StartTimer()
    {
        pauseTimer = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseTimer = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseTimer = false;
    }

    public void OpenCloseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            menuOpened = !menuOpened;
            if (menuOpened)
            {
                PauseGame();
                SceneManager.LoadScene(1, LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.UnloadSceneAsync(1);
                ResumeGame();
            }
        }
    }
}
