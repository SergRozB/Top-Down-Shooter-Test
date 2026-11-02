using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponsScript : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private bool isDone = true;
    [SerializeField] private bool hasCompletedAnimation = true;
    private Animator animator;
    private GameObject centralDataHub;

    [Header("Scripts and Related Variables")]
    private OtherPlayerFunctions playerFuncs;
    private EnemyScript enemyScript;
    private OtherData otherData;

    [Header("Gun Characteristics")]
    public Gun gun;
    private string gunName;
    [SerializeField] private RaycastHit2D bulletHit;
    public int bulletMinDamage;
    public int bulletMaxDamage;
    public float bulletSpeed;
    private int mags;
    [SerializeField] private GameObject fP; //Firing Point
    [SerializeField] private float fireRate;
    [SerializeField] private int magazineSize;
    private int currentAmmoCount;
    [SerializeField] private float reloadSpeed;
    private AudioClip pistolShootClip;
    private float bulletRange;
    private float gunSpread;
    private GameObject bullet;

    [Header("Muzzle Flash Characteristics")]
    private ParticleSystem muzzleFlashParticle;

    private void Awake()
    {
        centralDataHub = GameObject.FindGameObjectWithTag("CentralDataHub");
        otherData = centralDataHub.GetComponent<OtherData>();
        bullet = otherData.bulletGameObject;
        gun = otherData.guns[Random.Range(0, otherData.guns.Length)];
        enemyScript = GetComponent<EnemyScript>();
        animator = GetComponentInChildren<Animator>();
        muzzleFlashParticle = enemyScript.GetChildWithTag(gameObject, "MuzzleFlash").GetComponent<ParticleSystem>();
        pistolShootClip = otherData.enemyPistolShootClip;
        rb = GetComponent<Rigidbody2D>();
        UpdateGunVariables();
    }

    private void UpdateGunVariables()
    {
        gunName = gun.gunName;
        bulletSpeed = gun.bulletSpeed;
        bulletMinDamage = gun.bulletMinDamage;
        bulletMaxDamage = gun.bulletMaxDamage;
        magazineSize = gun.oneMagCapacity;
        reloadSpeed = gun.gunReloadSpeed;
        fireRate = gun.gunFireInterval;
        bulletSpeed = gun.bulletSpeed;
        currentAmmoCount = magazineSize;
        fP.transform.position = transform.TransformPoint(gun.firingPoint);
        muzzleFlashParticle.transform.position = transform.TransformPoint(gun.firingPoint);
        gunSpread = gun.spread;
    }

    public void EnemyAssignGunVariables(GunHolderScript gunHolder)
    {
        gunHolder.gunName = gunName;
        gunHolder.bulletSpeed = bulletSpeed;
        gunHolder.bulletMinDamage = bulletMinDamage;
        gunHolder.bulletMaxDamage = bulletMaxDamage;
        gunHolder.mags = gun.mags;
        gunHolder.oneMagCapacity = magazineSize;
        gunHolder.gunReloadSpeed = reloadSpeed;
        gunHolder.gunFireInterval = fireRate;
        gunHolder.currentGunAmmo = gun.oneMagCapacity;
        gunHolder.isAutomatic = gun.isAutomatic;
        gunHolder.firingPoint = gun.firingPoint;
        gunHolder.spread = gunSpread;
    }

    void Shoot()
    {
        if (isDone)
        {
            SoundManager.Instance.PlaySound(pistolShootClip);
            isDone = false;
            animator.SetTrigger("Shoot");
            muzzleFlashParticle.Play();
            EnemyBulletScript bulletScript = bullet.GetComponent<EnemyBulletScript>();
            bulletScript.originObject = gameObject;
            float angleSpread = CalculateAngle();
            Quaternion bulletRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, angleSpread));
            Instantiate(bullet, fP.transform.position, bulletRotation);
            bulletHit = Physics2D.Raycast(fP.transform.position, fP.transform.up);
            //Debug.DrawLine(fP.transform.position, bulletHit.point, Color.red, 5);
            if (currentAmmoCount > 0)
            {
                currentAmmoCount -= 1;
            }
            else
                Invoke("Reload", reloadSpeed);
            Invoke("ResetShot", fireRate);
        }
    }

    float CalculateAngle()
    {
        float angle = Random.Range(-gunSpread, gunSpread);
        return angle;
    }

    public void ReactionTime(bool isSudden)
    {
        if (isSudden)
        {
            Invoke("Shoot", CentralDataScript.suddenReactionTime);
        }

        else
        {
            Invoke("Shoot", CentralDataScript.generalReactionTime);
        }
    }

    void ResetShot()
    {
        isDone = true;
        animator.SetBool("isFiring", false);
        hasCompletedAnimation = true;
    }

    void Reload()
    {
        currentAmmoCount = magazineSize;
        mags--;
        gun.currentGunAmmo = currentAmmoCount;
        if (mags >= 0)
            gun.mags = mags;
    }
}