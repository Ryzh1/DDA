using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [System.Serializable]
    public class Gun
    {
        public string weaponName;
        public int damage;
        public float fireRate;
        public int magSize;
        public float reloadSpeed;
        public int index;
        
    }

    public Gun[] Guns;
    public Gun CurrentGun;
    public int CurrentMagSize;
    public int MaxAmmo;


    public Camera FpsCam;
    public ParticleSystem MuzzleFlash;

    private bool canFire;
    private float timer;

    public bool CanShoot;

    public float Accuracy;
    private int shotsHit;
    private int shotsFired;
    public int MaxShots;
    public GameObject currentFpsGun;
    public Animation gunAnim;
    public AudioManager audioManager;


    private void Start()
    {
        Accuracy = 100;
        CurrentGun = Guns[0];
        CurrentMagSize = CurrentGun.magSize;
        canFire = true;
        CanShoot = true;
        timer = 0;
        audioManager = FindObjectOfType<AudioManager>();

    }

    private void Update()
    {
        if(shotsFired > 0)
        {
            Accuracy = (shotsHit * 100) / shotsFired;
        }
        
        if (CurrentMagSize < CurrentGun.magSize && Input.GetKeyDown(KeyCode.R) && MaxAmmo != 0)
        {
            CanShoot = false;
            StartCoroutine(ReloadDelay());

        }

    }
    public void MuzzleFlashChange()
    {
        if (MuzzleFlash.name != currentFpsGun.name || MuzzleFlash == null)
        {
            gunAnim = currentFpsGun.gameObject.GetComponent<Animation>();
            MuzzleFlash = currentFpsGun.gameObject.GetComponent<ParticleSystem>();
        }
    }

    private void FixedUpdate()
    {
        Shooting(CanShoot);
    }

    private void Shooting(bool canShoot)
    {
        if (canShoot)
        {
            if (Input.GetButton("Fire1") && canFire && CurrentMagSize > 0)
            {
                canFire = false;
                Fire();
                CurrentMagSize -= 1;

            }
            else if (!canFire)
            {
                if (timer < CurrentGun.fireRate)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    canFire = true;
                    timer = 0;
                }
            }
        }

    }

    private void Fire()
    {
        MuzzleFlash.Play();
        gunAnim.Play();
        //GunSound.pitch = UnityEngine.Random.Range(0.5f, 1f);
        //GunSound.Play();
        audioManager.PlayAudio("gunsound", currentFpsGun.transform.position, 0.2f);
        RaycastHit hit;
        shotsFired++;
        MaxShots++;

        int layermask1 = 1 << 6;
        int layermask2 = 1 << 8;
        int layermask3 = 1 << 11;
        int finalmask = layermask1 | layermask2 | layermask3;
        finalmask = ~finalmask;

        if (Physics.Raycast(FpsCam.transform.position,FpsCam.transform.forward, out hit, 200f, finalmask))
        {
            Damageable target = hit.transform.GetComponent<Damageable>();
            if(target != null)
            {
                shotsHit++;
                target.TakeDamage(CurrentGun.damage, false);
            }
        }
    }


    
    IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(CurrentGun.reloadSpeed);

        var ammoleft = CurrentGun.magSize - CurrentMagSize;
        if((MaxAmmo - ammoleft) <= 0)
        {
            CurrentMagSize += MaxAmmo;
            MaxAmmo = 0;
        }
        else
        {
            MaxAmmo -= ammoleft;
            CurrentMagSize += ammoleft;
        }
        CanShoot = true;


    }

    public void ResetAccuracy()
    {
        shotsFired = 0;
        shotsHit = 0;
        Accuracy = 100;


    }


}
