using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    public Gun currentGun;
    public Gun[] guns;
    private int gunIndex;

    private void Start()
    {
        gunIndex = 0;
        guns = GetComponentsInChildren<Gun>();
        currentGun = guns[gunIndex];

        foreach (Gun gun in guns)
        {
            if (gun != currentGun)
            {
                gun.gameObject.SetActive(false);
            }
        }
    }

    public void SwitchGun()
    {
        gunIndex++;
        if (gunIndex >= guns.Length)
        {
            gunIndex = 0;
        }

        currentGun = guns[gunIndex];
        currentGun.gameObject.SetActive(true);
        foreach (Gun gun in guns)
        {
            if (gun != currentGun)
            {
                gun.gameObject.SetActive(false);
            }
        }
    }
}
