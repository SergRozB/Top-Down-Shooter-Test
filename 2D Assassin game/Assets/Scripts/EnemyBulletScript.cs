using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    private float speed;
    private int bulletMinDamage;
    private int bulletMaxDamage;
    EnemyWeaponsScript weaponScript;
    private Vector3 startingPosition;
    private ParticleSystem wallParticle;
    private ParticleSystem bloodSplatterParticle;
    Rigidbody2D rb;
    RaycastHit2D lastBulletHit;
    RaycastHit2D currentBulletHit;
    public ParticleSelfDestruct selfDestructScript;
    private LayerMask bulletMask;
    private OtherData otherData;
    private GameObject centralDataHub;
    public GameObject originObject;

    void Awake()
    {
        weaponScript = originObject.GetComponent<EnemyWeaponsScript>();
        centralDataHub = GameObject.FindGameObjectWithTag("CentralDataHub");
        otherData = centralDataHub.GetComponent<OtherData>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        speed = weaponScript.bulletSpeed;
        startingPosition = transform.position;
        bulletMinDamage = weaponScript.bulletMinDamage;
        bulletMaxDamage = weaponScript.bulletMaxDamage;
        wallParticle = otherData.wallHitParticle;
        bloodSplatterParticle = otherData.bloodSprayParticle;
        bulletMask = otherData.bulletMask;
        //currentBulletHit = Physics2D.Raycast(transform.position, transform.up, bulletMask);
    }

    void FixedUpdate()
    {
       // lastBulletHit = currentBulletHit;
        //currentBulletHit = Physics2D.Raycast(transform.position, transform.up, bulletMask);
       
        Vector3 rbPos = rb.position;
        rb.MovePosition(rbPos + (transform.up * speed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HAs collided ENemy");
        RaycastHit2D particleRay = Physics2D.Raycast(transform.position, transform.up, bulletMask);
        Debug.DrawLine(transform.position, particleRay.point, Color.red, 5);
        ParticleSystem collisionParticle;
        var difference = transform.position - startingPosition;
        var angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(angle, -90, 0); //rotation particle should be at
        Debug.Log("Player Hit:" + collision.tag);

        if (collision.transform.tag == "Player")
        {
            Debug.Log("E Hit player");
            OtherPlayerFunctions playerScript = collision.transform.gameObject.GetComponent<OtherPlayerFunctions>();
            int bulletDamage = Random.Range(bulletMinDamage, bulletMaxDamage);
            playerScript.TakeDamage(bulletDamage);
            collisionParticle = Instantiate(bloodSplatterParticle, particleRay.point, rotation); //creating blood particle at hit point and at the prev calculated rotation
            collisionParticle.gameObject.AddComponent<ParticleSelfDestruct>();
        }

        else
        {
            Debug.Log("E Hit wall");
            Debug.DrawLine(transform.position, lastBulletHit.point, Color.blue, 5);
            collisionParticle = Instantiate(wallParticle, particleRay.point, rotation); ; //creating wall hit particle at hit point and at the prev calculated rotation
            collisionParticle.gameObject.AddComponent<ParticleSelfDestruct>();
        }

        Destroy(gameObject);
    }
}
