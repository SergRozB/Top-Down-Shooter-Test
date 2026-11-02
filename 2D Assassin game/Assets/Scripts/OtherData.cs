using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherData : MonoBehaviour
{
    [Header("Enemy Miscellaneous")]
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public LayerMask bulletMask;
    public AnimationCurve rotationCurve;
    public ParticleSystem wallHitParticle;
    public ParticleSystem bloodSprayParticle;
    public GameObject bulletGameObject;
    public GameObject enemyRagdoll;
    public GameObject checkObject;

    [Header("EnemyAI Stuff")]
    public string[] behaviours;

    [Header("Enemy Sprites")]
    public Sprite[] fatalHeadShots;
    public Sprite[] fatalBodyShots;
    public Sprite[] lArmInjury;
    public Sprite[] rArmInjury;
    public Sprite[] lForearmInjury;
    public Sprite[] rForearmInjury;
    public Sprite[] lLegInjury;
    public Sprite[] rLegInjury;
    public Sprite[] bodyInjury;
    public Sprite[] doubleBody; //Two shots to the torso
    public Sprite enemyM4A1Sprite;
    public Sprite enemyGlockSprite;

    [Header("Enemy Audio")]
    public AudioClip enemyPistolShootClip;

    [Header("Weapons")]
    public Gun[] guns;
}
