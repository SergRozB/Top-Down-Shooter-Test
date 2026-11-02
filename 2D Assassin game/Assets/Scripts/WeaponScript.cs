using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [Header("Key Codes")]
    KeyCode shootKey = KeyCode.Mouse0;
    KeyCode reloadKey = KeyCode.R;
    KeyCode meleeKey = KeyCode.Mouse1;
    KeyCode pickupKey = KeyCode.Mouse2;

    [Header("Gun Characteristics")]
    public Transform bullet;
    public string gunName;
    public float bulletSpeed;
    [SerializeField] private RaycastHit2D bulletHit; 
    public int bulletMinDamage;
    public int bulletMaxDamage;
    public GameObject fP; //Firing Point, where bullet is shot from
    public int mags;
    [SerializeField] private int oneMagCapacity; 
    public int currentGunAmmo;
    [SerializeField] private float gunReloadSpeed;
    [SerializeField] private float gunFireInterval;
    public bool isAutomatic;
    public float gunSpread;
    public Gun gun;
    public GunHolderScript gunHolder;

    [Header("Other Gun Variables")]
    public AudioClip pistolShoot;
    public bool isDone = true; // is done shooting
    [SerializeField] private bool keyPressed, isReloading;
    public LayerMask bulletMask;

    [Header("Particles")]
    private ParticleSystem muzzleFlashParticle;
    public ParticleSystem bloodSplatterParticle;
    public ParticleSystem wallParticle;

    [Header("Melee Characteristics")]
    public float meleeRange = 2f;
    public float meleeAngle = 90f;
    [SerializeField]
    private float stunDuration;

    [Header("Other")]
    Rigidbody2D rb;
    public EnemyScript enemyScript;
    private Animator animator;
    private LineRenderer lineRenderer;
    private RaycastHit2D lineHit;
    private GameObject wallHit;
    public LayerMask enemyMask;
    public LayerMask obstructionMask;
    public LayerMask gunEquipMask;
    [SerializeField] private float lineLength;
    private OtherPlayerFunctions otherFunctions;

    private void Start()
    {
        muzzleFlashParticle = enemyScript.GetChildWithTag(fP, "MuzzleFlash").GetComponent<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();   
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (gun != null) 
        {
            UpdateGunVariables(gun);
            currentGunAmmo = oneMagCapacity;
        }
        otherFunctions = GetComponent<OtherPlayerFunctions>();
    }

    private void Update()
    {
        InputControls();
        if (gun != null)
        {
            lineRenderer.enabled = true;
            DrawLine();
            gunHolder.gunName = "NoWeapon";
        }

        else
        {
            lineRenderer.enabled = false;
        }
    }

    void InputControls()
    {
        if (isAutomatic) keyPressed = Input.GetKey(shootKey);
        else keyPressed = Input.GetKeyDown(shootKey);

        if (keyPressed && isDone && !isReloading && currentGunAmmo > 0 && gun != null)
            Shoot();

        if (Input.GetKeyDown(reloadKey) && mags > 0 && !isReloading)
            Reload();

        if (Input.GetKeyDown(meleeKey))
            Melee();

        if (Input.GetKeyDown(pickupKey))
            TryEquipGun();
    }

    private void TryEquipGun()
    {
        List<Transform> listOfGuns = new List<Transform>();
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, meleeRange);
        foreach (Collider2D collider in rangeChecks)
        {
            if (collider.tag == "Gun")
            {
                float distanceToTarget = Vector2.Distance(transform.position, collider.transform.position);
                Vector2 directionToTarget = collider.transform.position - transform.position;

                //Raycast to check if path betweem player and gun is unobstructed
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    listOfGuns.Add(collider.transform);
                }
            }
        }

        if (listOfGuns.Count > 0)
        {
            Transform closestGun = listOfGuns[0];

            foreach (Transform t in listOfGuns)
            {
                float distanceBetweenNew = Vector2.Distance(t.position, transform.position);
                float distanceBetweenOld = Vector2.Distance(closestGun.position, transform.position);
                if (distanceBetweenNew > distanceBetweenOld)
                {
                    closestGun = t;
                }
            }

            if (gun != null)
                RemoveGun();
            gunHolder = closestGun.GetComponent<GunHolderScript>();
            gun = gunHolder.gun; //gets new gun from holder script
            Destroy(closestGun.gameObject);
            UpdateGunVariables(gunHolder);
            otherFunctions.playerSpriteManager(gun.gunName);
        }

        else
        {
            if (gun != null)
                RemoveGun();
            otherFunctions.playerSpriteManager("NoWeapon");
            gun = null;
        }
    }

    public void RemoveGun()
    {
        Rigidbody2D gunRb = Instantiate(gun.gunObject, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
        gunRb.velocity = transform.up * 20;
        GunHolderScript holder = gunRb.gameObject.GetComponent<GunHolderScript>();
        AssignGunVariables(holder);
    }

    public void UpdateGunVariables(GunHolderScript gun)
    {
        gunName = gun.gunName;
        bulletSpeed = gun.bulletSpeed;
        bulletMinDamage = gun.bulletMinDamage;
        bulletMaxDamage = gun.bulletMaxDamage;
        mags = gun.mags;
        oneMagCapacity = gun.oneMagCapacity;
        gunReloadSpeed = gun.gunReloadSpeed;
        gunFireInterval = gun.gunFireInterval;
        isAutomatic = gun.isAutomatic;
        currentGunAmmo = gun.currentGunAmmo;
        fP.transform.position = transform.TransformPoint(gun.firingPoint);
        muzzleFlashParticle.transform.position = transform.TransformPoint(gun.firingPoint);
        gunSpread = gun.spread;
    }

    public void UpdateGunVariables(Gun gun)
    {
        gunName = gun.gunName;
        bulletSpeed = gun.bulletSpeed;
        bulletMinDamage = gun.bulletMinDamage;
        bulletMaxDamage = gun.bulletMaxDamage;
        mags = gun.mags;
        oneMagCapacity = gun.oneMagCapacity;
        gunReloadSpeed = gun.gunReloadSpeed;
        gunFireInterval = gun.gunFireInterval;
        isAutomatic = gun.isAutomatic;
        currentGunAmmo = gun.currentGunAmmo;
        fP.transform.position = transform.TransformPoint(gun.firingPoint);
        muzzleFlashParticle.transform.position = transform.TransformPoint(gun.firingPoint);
        gunSpread = gun.spread;
    }

    private void AssignGunVariables(GunHolderScript gunHolder)
    {
        gunHolder.playerDropped = true;
        gunHolder.gunName = gunName;
        gunHolder.bulletSpeed = bulletSpeed;
        gunHolder.bulletMinDamage = bulletMinDamage;
        gunHolder.bulletMaxDamage = bulletMaxDamage;
        gunHolder.mags = mags;
        gunHolder.oneMagCapacity = oneMagCapacity;
        gunHolder.gunReloadSpeed = gunReloadSpeed;
        gunHolder.gunFireInterval = gunFireInterval;
        gunHolder.isAutomatic = isAutomatic;
        gunHolder.currentGunAmmo = currentGunAmmo;
        gunHolder.firingPoint = gun.firingPoint;
        gunHolder.spread = gunSpread;
    }

    void Shoot()
    {
        SoundManager.Instance.PlaySound(pistolShoot);
        isDone = false;
        animator.SetTrigger("Shoot");
        muzzleFlashParticle.Play();
        float angleSpread = CalculateAngle();
        Quaternion bulletRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, angleSpread));
        Instantiate(bullet, fP.transform.position, bulletRotation);
        bulletHit = Physics2D.Raycast(fP.transform.position, fP.transform.up);
        //Debug.DrawLine(fP.transform.position, bulletHit.point, Color.red, 5);
        if (currentGunAmmo > 0)
        {
            currentGunAmmo -= 1;
        }
        Invoke("ResetShot", gunFireInterval);
        //animator.SetBool("isShooting", false);
    }

    void Reload()
    {
        isReloading = true;
        Invoke("NewMag", gunReloadSpeed);
    }

    float CalculateAngle()
    {
        float angle = Random.Range(-gunSpread, gunSpread);
        return angle;
    }
    void NewMag()

    {
        currentGunAmmo = oneMagCapacity;
        mags--;
        isReloading = false;
    }

    void ResetShot()
    {
        isDone = true;
    }

    void Melee()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, meleeRange, enemyMask);
        foreach (Collider2D enemyCollider in rangeChecks)
        {
            Transform enemy = enemyCollider.transform;
            Vector2 directionToTarget = (enemy.position - transform.position).normalized; //Angle between enemy and player

            if (Vector2.Angle(transform.up, directionToTarget) < meleeAngle / 2) //If the angle above is in the melee range
            {
                float distanceToTarget = Vector2.Distance(transform.position, enemy.position);

                //Raycast to check if path betweem player and enemy is unobstructed
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    EnemyGeneral enemyScript = enemy.gameObject.GetComponent<EnemyGeneral>();
                    //Calls the stun function in the enemy script with the stun duration as input
                    enemyScript.TakeDamage(enemyScript.health);
                }
            }
        }
    }

    private void DrawLine()
    {
        lineRenderer.SetPosition(0, fP.transform.position);
        Vector3 lineEndPos = fP.transform.position + fP.transform.up * lineLength;
        lineRenderer.SetPosition(1, lineEndPos);
    }
}
