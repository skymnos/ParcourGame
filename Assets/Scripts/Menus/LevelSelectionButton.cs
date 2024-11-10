using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionButton : MonoBehaviour
{
    public int index;

    public void SelectLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(index + 1, LoadSceneMode.Single);
    }
}
