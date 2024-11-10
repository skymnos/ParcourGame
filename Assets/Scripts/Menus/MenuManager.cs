using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject levelSelectionCanvas;
    [SerializeField] private GameObject settingsCanvas;

    [SerializeField] private GameObject menuFirst, settingsFirst, levelSelectionFirst; 

    private void Awake()
    {
        menuCanvas.SetActive(true);
        levelSelectionCanvas.SetActive(false);
        settingsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(menuFirst);
    }

    private void GoToMenu()
    {
        menuCanvas.SetActive(true);
        levelSelectionCanvas.SetActive(false);
        settingsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(menuFirst);
    }

    private void GoToLevelSelection()
    {
        menuCanvas.SetActive(false);
        levelSelectionCanvas.SetActive(true);
        settingsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(levelSelectionFirst);
    }

    private void GoToSettings()
    {
        menuCanvas.SetActive(false);
        levelSelectionCanvas.SetActive(false);
        settingsCanvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(settingsFirst);
    }

    private void GoToGame()
    {
        menuCanvas.SetActive(false);
        levelSelectionCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }

    public void ReturnPressed()
    {
        GoToMenu();
    }

    public void SettingsPressed()
    {
        GoToSettings();
    }

    public void LevelSelectionPressed()
    {
        GoToLevelSelection();
    }

    public void ReturnToGame()
    {
        GoToGame();
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }
}
