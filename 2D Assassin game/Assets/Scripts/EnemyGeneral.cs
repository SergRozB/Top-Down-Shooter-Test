using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneral : MonoBehaviour
{
    [Header("Other Scripts/Game Objects")]
    private GameObject player;
    private GameObject centralDataHub;
    private OtherData otherData;
    private EnemyWeaponsScript enemyWeaponsScript;
    private Animator animator;

    [Header("Enemy Stats")]
    public float health = 10;

    private int shotsTaken;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        centralDataHub = GameObject.FindGameObjectWithTag("CentralDataHub");
        otherData = centralDataHub.GetComponent<OtherData>();
        enemyWeaponsScript = GetComponent<EnemyWeaponsScript>();
        health = CentralDataScript.enemyHealth;
        animator = GetComponentInChildren<Animator>();
        Invoke("enemySpriteManager", 0.1f);
    }

    public GameObject GetChildWithTag(GameObject parent, string tag) //for finding ONE gameobject with a unique tag
    {
        foreach (Transform tr in parent.transform)
        {
            if (tr.tag == tag)
            {
                return tr.gameObject;
            }

            foreach (Transform tr2 in tr)
            {
                if (tr2.tag == tag)
                {
                    return tr2.gameObject;
                }
            }
        }
        return null;
    }

    public void TakeDamage(float damage)
    {
        shotsTaken++;
        health -= damage;
        if (health <= 0f)
        {
            var newRot = transform.rotation * Quaternion.Inverse(Quaternion.Euler(0, 0, -180));
            GameObject ragdoll = Instantiate(otherData.enemyRagdoll, transform.position + new Vector3(0, 0, 1), newRot);
            RagdollInjuries(ragdoll);
            Vector2 directionFromPlayer = ragdoll.transform.position - player.transform.position;
            Rigidbody2D torso = GetChildWithTag(ragdoll, "Torso").GetComponent<Rigidbody2D>();
            float force = UnityEngine.Random.Range(CentralDataScript.minRagdollForce, CentralDataScript.maxRagdollForce);
            torso.velocity += directionFromPlayer * force;
            EnemyWeaponsScript enemyWeaponsScript = gameObject.GetComponent<EnemyWeaponsScript>();
            GameObject prevGun = Instantiate(enemyWeaponsScript.gun.gunObject, transform.position, transform.rotation);
            GunHolderScript prevGunScript = prevGun.GetComponent<GunHolderScript>();
            enemyWeaponsScript.EnemyAssignGunVariables(prevGunScript);
            Destroy(this.gameObject);
        }
    }

    private void enemySpriteManager()
    {
        SpriteRenderer spriteRenderer = GetChildWithTag(gameObject, "EnemyBody").GetComponent<SpriteRenderer>();
        Debug.Log("Disen happened");
        switch (enemyWeaponsScript.gun.gunName)
        {
            case "Glock":
                {
                    Sprite glockSprite = otherData.enemyGlockSprite;
                    spriteRenderer.sprite = glockSprite;
                    animator.SetInteger("Weapon", 1);
                    break;
                }

            case "M4":
                {
                    Sprite m4Sprite = otherData.enemyM4A1Sprite;
                    spriteRenderer.sprite = m4Sprite;
                    animator.SetInteger("Weapon", 2);
                    break;
                }
        }
    }


    void RagdollInjuries(GameObject ragdoll)
    {
        //Meant to be used with ragdolls with specific tags for each limb
        if (shotsTaken == 1)
        {
            int fiftyFifty = UnityEngine.Random.Range(0, 2);
            if (fiftyFifty == 0)
            {
                SpriteRenderer head = GetChildWithTag(ragdoll, "Head").GetComponent<SpriteRenderer>();
                head.sprite = otherData.fatalHeadShots[UnityEngine.Random.Range(0, otherData.fatalHeadShots.Length)]; //Random sprite from fata head shots sprite list
            }

            else
            {
                SpriteRenderer torso = GetChildWithTag(ragdoll, "Torso").GetComponent<SpriteRenderer>();
                torso.sprite = otherData.fatalBodyShots[UnityEngine.Random.Range(0, otherData.fatalBodyShots.Length)]; //Random sprite from fata head shots sprite list
            }
        }

        else
        {
            int fiftyFifty = UnityEngine.Random.Range(0, 2);
            if (fiftyFifty == 0)
            {
                SpriteRenderer head = GetChildWithTag(ragdoll, "Head").GetComponent<SpriteRenderer>();
                head.sprite = otherData.fatalHeadShots[UnityEngine.Random.Range(0, otherData.fatalHeadShots.Length)]; //Random sprite from fata head shots sprite list
                int chooseOtherLimb = UnityEngine.Random.Range(0, 7);
                switch (chooseOtherLimb)
                {
                    case 0:
                        SpriteRenderer torso = GetChildWithTag(ragdoll, "Torso").GetComponent<SpriteRenderer>();
                        torso.sprite = otherData.bodyInjury[UnityEngine.Random.Range(0, otherData.bodyInjury.Length)];
                        break;
                    case 1:
                        SpriteRenderer armL = GetChildWithTag(ragdoll, "ArmL").GetComponent<SpriteRenderer>();
                        armL.sprite = otherData.lArmInjury[UnityEngine.Random.Range(0, otherData.lArmInjury.Length)];
                        break;
                    case 2:
                        SpriteRenderer armR = GetChildWithTag(ragdoll, "ArmR").GetComponent<SpriteRenderer>();
                        armR.sprite = otherData.rArmInjury[UnityEngine.Random.Range(0, otherData.rArmInjury.Length)];
                        break;
                    case 3:
                        SpriteRenderer forearmL = GetChildWithTag(ragdoll, "ForearmL").GetComponent<SpriteRenderer>();
                        forearmL.sprite = otherData.lForearmInjury[UnityEngine.Random.Range(0, otherData.lForearmInjury.Length)];
                        break;
                    case 4:
                        SpriteRenderer forearmR = GetChildWithTag(ragdoll, "ForearmR").GetComponent<SpriteRenderer>();
                        forearmR.sprite = otherData.rForearmInjury[UnityEngine.Random.Range(0, otherData.rForearmInjury.Length)];
                        break;
                    case 5:
                        SpriteRenderer legL = GetChildWithTag(ragdoll, "ThighL").GetComponent<SpriteRenderer>();
                        legL.sprite = otherData.lLegInjury[UnityEngine.Random.Range(0, otherData.lLegInjury.Length)];
                        break;
                    case 6:
                        SpriteRenderer legR = GetChildWithTag(ragdoll, "ThighR").GetComponent<SpriteRenderer>();
                        legR.sprite = otherData.rLegInjury[UnityEngine.Random.Range(0, otherData.rLegInjury.Length)];
                        break;
                }
            }

            else
            {
                SpriteRenderer torso = GetChildWithTag(ragdoll, "Torso").GetComponent<SpriteRenderer>();
                torso.sprite = otherData.fatalBodyShots[UnityEngine.Random.Range(0, otherData.fatalBodyShots.Length)]; //Random sprite from fata head shots sprite list
                int chooseOtherLimb = UnityEngine.Random.Range(0, 7);
                switch (chooseOtherLimb)
                {
                    case 0:
                        SpriteRenderer doubleTorso = GetChildWithTag(ragdoll, "Torso").GetComponent<SpriteRenderer>();
                        doubleTorso.sprite = otherData.doubleBody[UnityEngine.Random.Range(0, otherData.doubleBody.Length)];
                        break;
                    case 1:
                        SpriteRenderer armL = GetChildWithTag(ragdoll, "ArmL").GetComponent<SpriteRenderer>();
                        armL.sprite = otherData.lArmInjury[UnityEngine.Random.Range(0, otherData.lArmInjury.Length)];
                        break;
                    case 2:
                        SpriteRenderer armR = GetChildWithTag(ragdoll, "ArmR").GetComponent<SpriteRenderer>();
                        armR.sprite = otherData.rArmInjury[UnityEngine.Random.Range(0, otherData.rArmInjury.Length)];
                        break;
                    case 3:
                        SpriteRenderer forearmL = GetChildWithTag(ragdoll, "ForearmL").GetComponent<SpriteRenderer>();
                        forearmL.sprite = otherData.lForearmInjury[UnityEngine.Random.Range(0, otherData.lForearmInjury.Length)];
                        break;
                    case 4:
                        SpriteRenderer forearmR = GetChildWithTag(ragdoll, "ForearmR").GetComponent<SpriteRenderer>();
                        forearmR.sprite = otherData.rForearmInjury[UnityEngine.Random.Range(0, otherData.rForearmInjury.Length)];
                        break;
                    case 5:
                        SpriteRenderer legL = GetChildWithTag(ragdoll, "ThighL").GetComponent<SpriteRenderer>();
                        legL.sprite = otherData.lLegInjury[UnityEngine.Random.Range(0, otherData.lLegInjury.Length)];
                        break;
                    case 6:
                        SpriteRenderer legR = GetChildWithTag(ragdoll, "ThighR").GetComponent<SpriteRenderer>();
                        legR.sprite = otherData.rLegInjury[UnityEngine.Random.Range(0, otherData.rLegInjury.Length)];
                        break;
                }
            }
        }
    }
}
