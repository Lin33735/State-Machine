using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject StartMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject PlayerUI;

    public bool gamePaused;

    void Awake()
    {
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        PauseMenu.SetActive(false);
        PlayerUI.SetActive(true);
        Time.timeScale = 1.0f;
        gamePaused = false;
    }

    void Pause()
    {
        PauseMenu.SetActive(true);
        PlayerUI.SetActive(false);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void ChangeSceneByName(string name)
    {
        if (name != null)
        {
            SceneManager.LoadScene(name);
        }
    }

    public void ResumeButton()
    {
        Resume();
    }
}
