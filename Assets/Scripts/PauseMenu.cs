using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameObject pauseMenu;
    public static bool GamePaused = false;
    public void PauseGame()
    {
        Time.timeScale = 0f;

        pauseMenu = gameObject.transform.GetChild(0).gameObject;
        pauseMenu.SetActive(true);
        GamePaused = true;
    }

    public void ResumeGame()
    {
        GamePaused = false;
        pauseMenu = gameObject.transform.GetChild(0).gameObject;
        pauseMenu.SetActive(false);

        Time.timeScale = GameSession.GameTimeScale;
    }
}
