using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool briefcaseCollected = false;
    public GameObject winZone;
    
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

    
}
