using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDetectionScript : MonoBehaviour
{
    private GameObject player;

    [Header("Other Scripts")]
    private EnemyScript enemyScript;
    private WeaponScript weaponScript;

    public bool detectedPlayer = false;
    private float distanceToPlayer;

    private void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        weaponScript = player.GetComponent<WeaponScript>();
    }
    private void Update()
    {
        DetectionManager();
        enemyScript.hasDetectedPlayer = detectedPlayer;
        distanceToPlayer = Vector2.Distance(player.transform.position, gameObject.transform.position);
    }

    private void DetectionManager()
    {
        if (weaponScript.isDone == false && distanceToPlayer < CentralDataScript.maxShotsHeardRange)
        {
            detectedPlayer = true;
            enemyScript.ResetVariables();
            enemyScript.lastPlayerLocation = player.transform.position;
            enemyScript.hasDetectedPlayer = detectedPlayer;
        }
    }
}
