using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CentralDataScript
{

    [Header("General Enemy Info")]
    public static float enemyRoamSpeed = 450;
    public static float enemyHuntSpeed = 750;
    public static float enemyHealth = 12;

    [Header("EnemyAI Info")]
    public static float aggresiveDistToTarget = 10; //Max distance to player AI can get if aggressive
    public static float cautiousDistToTarget = 25; //Max distance to player AI can get if cautious
    public static float maxShotsHeardRange = 45f; //The furthest away shots can be heard by the AI.
    public static float suddenReactionTime = 0.6f;
    public static float generalReactionTime = 0.2f;
    public static float maxWanderTime = 4;
    public static float minWanderTime = 2;
    public static float wanderLimit = 10; //Max distance AI can wander from current position
    public static float changeDist = 3;

    [Header("Enemy FOV Values")]
    public static float radius = 30;
    public static float angle = 160;

    [Header("Enemy Weapon Values")]
    public static float fireRate = 0.2f;
    public static float enemyBulletRange = 30;
    public static float engagementDist = 25;

    [Header("Miscellaneous Enemy Info")]
    public static float exactPlayerPosLimit = 30;
    public static float rotateSpeed = 10;
    public static float minRagdollForce = 30;
    public static float maxRagdollForce = 60;

}       
