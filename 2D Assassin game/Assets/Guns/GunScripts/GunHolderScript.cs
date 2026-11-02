using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolderScript : MonoBehaviour
{
    public Gun gun;
    public string gunName ;
    public float bulletSpeed;
    public int bulletMinDamage;
    public int bulletMaxDamage;
    public int mags;
    public int oneMagCapacity;
    public int currentGunAmmo;
    public float gunReloadSpeed;
    public float gunFireInterval;
    public bool isAutomatic;
    public Vector3 firingPoint;
    public float spread;
    public bool playerDropped = false;

    private void Awake()
    {
        if (!playerDropped)
        { 
            gunName = gun.name;
            bulletSpeed = gun.bulletSpeed;
            bulletMinDamage = gun.bulletMinDamage;
            bulletMaxDamage = gun.bulletMaxDamage;
            mags = gun.mags;
            oneMagCapacity = gun.oneMagCapacity;
            currentGunAmmo = gun.currentGunAmmo;
            gunReloadSpeed = gun.gunReloadSpeed;
            gunFireInterval = gun.gunFireInterval;
            isAutomatic = gun.isAutomatic;
            firingPoint = gun.firingPoint;
            spread = gun.spread;
        }
    }
}
