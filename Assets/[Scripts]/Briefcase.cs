using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Briefcase : Interactable
{
    public GameObject briefcase;
    public PlayerUI playerUI;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        playerUI.UpdateObjectiveText("Return To The Helicopter");
        gameManager.briefcaseCollected = true;
        Destroy(briefcase);
    }
}
