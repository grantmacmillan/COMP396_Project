using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool briefcaseCollected = false;
    public GameObject winZone;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<SoundManager>().Play("WhiteNoise");
    }

    // Update is called once per frame
    void Update()
    {
        if (briefcaseCollected)
        {
            winZone.SetActive(true);
            
        }
    }

    public void Pause(GameObject obj)
    {
        obj.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        InputManager.Instance.SwitchActionMap();
        Time.timeScale = 0f;
    }

    public void Resume(GameObject obj)
    {
        obj.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InputManager.Instance.SwitchActionMap();
        Time.timeScale = 1f;
    }
}
