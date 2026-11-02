using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float speed;
    private int bulletMinDamage;
    private int bulletMaxDamage;
    WeaponScript weaponScript;
    private GameObject player;
    private Vector3 startingPosition;
    private ParticleSystem wallParticle;
    private ParticleSystem bloodSplatterParticle;
    Rigidbody2D rb;
    RaycastHit2D lastBulletHit;
    RaycastHit2D currentBulletHit;
    public ParticleSelfDestruct selfDestructScript;
    private LayerMask bulletMask;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        weaponScript = player.GetComponent<WeaponScript>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        speed = weaponScript.bulletSpeed;
        startingPosition = player.transform.position;
        bulletMinDamage = weaponScript.bulletMinDamage;
        bulletMaxDamage = weaponScript.bulletMaxDamage;
        wallParticle = weaponScript.wallParticle;
        bloodSplatterParticle = weaponScript.bloodSplatterParticle;
        bulletMask = weaponScript.bulletMask;
        currentBulletHit = Physics2D.Raycast(transform.position, transform.up, bulletMask);
    }

    void FixedUpdate()
    {
        lastBulletHit = currentBulletHit;
        currentBulletHit = Physics2D.Raycast(transform.position, transform.up, bulletMask);
        Debug.DrawLine(transform.position, lastBulletHit.point, Color.red);
        Vector3 rbPos = rb.position;
        rb.MovePosition(rbPos + (transform.up * speed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HAs collided");
        ParticleSystem collisionParticle;
        var difference = transform.position - startingPosition;
        var angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(angle, -90, 0); //rotation particle should be at
        Debug.Log("Player Hit:" + collision.tag);

        if (collision.transform.tag == "Enemy")
        {
            Debug.Log("Hit enemy");
            EnemyGeneral enemyScript = collision.transform.gameObject.GetComponent<EnemyGeneral>();
            int bulletDamage = Random.Range(bulletMinDamage, bulletMaxDamage);
            enemyScript.TakeDamage(bulletDamage);
            collisionParticle = Instantiate(bloodSplatterParticle, lastBulletHit.point, rotation); //creating blood particle at hit point and at the prev calculated rotation
            collisionParticle.gameObject.AddComponent<ParticleSelfDestruct>();
        }

        else
        {
            Debug.Log("Hit wall");
            Debug.DrawLine(transform.position, lastBulletHit.point, Color.blue, 5);
            collisionParticle = Instantiate(wallParticle, lastBulletHit.point, rotation); ; //creating wall hit particle at hit point and at the prev calculated rotation
            collisionParticle.gameObject.AddComponent<ParticleSelfDestruct>();
        }

        Destroy(gameObject);
    }
}
