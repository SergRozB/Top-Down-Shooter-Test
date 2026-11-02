using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string gunName;
    public GameObject gunObject;
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
}
