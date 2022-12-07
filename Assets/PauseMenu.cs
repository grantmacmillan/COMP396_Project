using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.onGround.Pause.triggered)
        {
            if (pauseMenu.activeSelf)
            {
                GameManager.Instance.Resume(pauseMenu);
            }
            else
            {
                GameManager.Instance.Pause(pauseMenu);
            }
        }
    }

    public void Resume()
    {
        GameManager.Instance.Resume(pauseMenu);
    }

    public void MainMenu()
    {
        GameManager.Instance.Resume(pauseMenu);
        SceneManager.LoadScene("MainMenu");
    }
}
