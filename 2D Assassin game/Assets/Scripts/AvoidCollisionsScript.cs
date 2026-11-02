using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCollisionsScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isColliding = false;
    private Collider2D collider;
    [SerializeField]
    private float avoidanceForce = 10f;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isColliding)
        {
            Vector2 directionToCollider = (rb.position - (Vector2)collider.transform.position).normalized;
            Debug.DrawRay(rb.position, directionToCollider, Color.red);
            Debug.DrawRay(rb.position, directionToCollider * -1, Color.blue);
            rb.AddForce((directionToCollider * -1) * avoidanceForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            collider = other.GetComponent<Collider2D>();
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }
}
