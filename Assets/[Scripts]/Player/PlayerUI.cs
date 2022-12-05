using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI objectiveText;
    // Start is called before the first frame update
    void Start()
    {
        objectiveText.text = "Locate & Collect The Briefcase";
    }

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    public void UpdateObjectiveText(string objectiveMessage)
    {
        objectiveText.text = objectiveMessage;
    }
}
