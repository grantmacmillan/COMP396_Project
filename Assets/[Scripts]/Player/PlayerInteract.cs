using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    public float distance = 4f;
    public LayerMask mask;
    private PlayerUI playerUI;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerLook>().camera;
        playerUI = GetComponent<PlayerUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;

        //only run if raycast hits something
        if(Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (InputManager.Instance.onGround.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
