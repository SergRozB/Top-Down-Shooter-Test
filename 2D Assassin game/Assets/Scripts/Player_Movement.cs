using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("Speed Values")]
    [SerializeField]
    private float walkSpeed = 1f;
    [SerializeField]
    private float runSpeed = 2f;
    [SerializeField]
    private float speed;

    [Header("Movement Conditions")]
    public bool isRunning = false;
    public bool isWalking = false;
    [SerializeField]
    private bool isMoving = false;

    [Header("Key Codes")]
    KeyCode walkKey = KeyCode.LeftShift;

    [Header("Audio")]
    [SerializeField]
    private AudioClip[] walkingSounds;
    [SerializeField]
    private AudioClip walkingClip;
    [SerializeField]
    private AudioClip runningClip;
    private AudioSource movementSource;
    [SerializeField]
    private float walkFreq = 0.5f;
    private bool hasFinishedAudioLoop = true;
    [SerializeField]
    private bool hasStartedWalking = false;

    [Header("Other")]
    Rigidbody2D rb;
    public Vector2 moveDirection;
    public Animator legAnimator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckForSpeed();
        MovementState();
        MovementAudio();
    }

    private void FixedUpdate()
    {
        LookToCursor();
        BasicInput();
    }

    void BasicInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(x, y);
        rb.velocity = moveDirection * speed;
    }

    void LookToCursor()
    {
        var difference = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
    }

    void MovementState()
    {

        if (Input.GetKey(walkKey) && isMoving)
        {
            isWalking = true;
            isRunning = false;
        }

        else if (isMoving)
        {
            isWalking = false;
            isRunning = true;
        }

        else
        {
            isWalking = false;
            isRunning = false;
        }

        if (isRunning == true)
        {
            speed = runSpeed;
        }

        if (isWalking == true)
        {
            speed = walkSpeed;
        }
    }

    void CheckForSpeed()
    {
        if (rb.velocity.magnitude > 0f)
        {
            isMoving = true;
            legAnimator.SetBool("isMoving", true);
        }

        else
        {
            isMoving = false;
            legAnimator.SetBool("isMoving", false);
        }
    }

    private void MovementAudio()
    {
        if (isMoving)
        {
            movementSource.enabled = true;
            if (isWalking && movementSource.clip != walkingClip)
            {
                movementSource.clip = walkingClip;
                movementSource.Play();
            }

            else if (isRunning && movementSource.clip != runningClip)
            {
                movementSource.clip = runningClip;
                movementSource.Play();
            }

            //movementSource.playOnAwake = true;
        }

        else
        {
            movementSource.enabled = false;
        }
    }
}
