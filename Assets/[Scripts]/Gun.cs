using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    [Header("Unity")]
    public GameObject bulletPrefab;
    public Camera playerCam;
    public Transform bulletSpawnPoint;
    public ParticleSystem muzzleFlash;

    [Header("Bullet Stats")] 
    public float speed;
    public float upwardSpeed;

    [Header("Gun Stats")] 
    public string name;
    public float fireRate;
    public float spread;
    public float bulletsPerTap;
    public float fireRateTap;
    public bool automatic;

    private int bulletsShot;
    private bool shooting, canFire = true;

    public bool allowInvoke = true;

    private void Awake()
    {
        playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
    }
    public void CheckInput(InputAction action)
    {
        if (automatic)
            shooting = action.IsPressed();
        else
            shooting = action.WasPerformedThisFrame();

        if (shooting && canFire)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        //FindObjectOfType<SoundManager>().Play("RifleFire");
        canFire = false;
        bulletsShot++;

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (spread < 0.3)
        {
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(75);
        }
        else
        {
            targetPoint = ray.GetPoint(10);
        }
        

        Vector3 direction = targetPoint - bulletSpawnPoint.position;

        float xSpread = Random.Range(-spread, spread);
        float ySpread = Random.Range(-spread, spread);
        float zSpread = Random.Range(-spread, spread);

        Vector3 directionWithSpread = direction + new Vector3(xSpread, ySpread, zSpread);

        GameObject currentBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * speed, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(playerCam.transform.up * upwardSpeed, ForceMode.Impulse);

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (allowInvoke)
        {
            Invoke("ResetShot", fireRate);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap)
        {
            Invoke("Shoot", fireRateTap);
        }
    }

    private void ResetShot()
    {
        canFire = true;
        allowInvoke = true;
    }
}
