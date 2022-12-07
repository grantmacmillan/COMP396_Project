using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public List<GameObject> levels;
    // Start is called before the first frame update
    void Start()
    {
        String level = PlayerPrefs.GetString("scene");

        if (level == "Level1")
        {
            levels.ElementAt(0).SetActive(true);
        }else if (level == "Level2")
        {
            levels.ElementAt(1).SetActive(true);
        }
        else if(level == "Level3")
        {
            levels.ElementAt(2).SetActive(true);
        }
        else
        {
            levels.ElementAt(0).SetActive(true);
        }
    }
}
