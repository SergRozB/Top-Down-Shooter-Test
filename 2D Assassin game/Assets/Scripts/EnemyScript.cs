using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    [Header("FOV values")]
    public float radius;
    [Range(0, 360)]
    public float angle;

    [Header("Enemy AI")]
    public float nextWaypointDist;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    Seeker seeker;
    private GameObject target;
    public bool canSeePlayer = false;
    public bool hasDetectedPlayer = false; //Player has been seen or heard but isn't currently in sight. False when the location player was last seen or heard at is checked and the player isn't there
    [SerializeField] float maxDistanceToTarget; //Closest distance AI can go to player.
    public Vector2 lastPlayerLocation;
    [SerializeField] private bool onSetPath = true;
    [SerializeField] private bool hasMadeSearchList = false;
    public bool oldMethod;
    public Transform[] patrolPoints;
    private int currentPoint = 0;
    private float distanceToTarget;
    [SerializeField] private string aiState = "IdlePath";
    [SerializeField] private string aiBehaviour;
    public bool isChecking = false;
    public bool stopMoving = false;
    private Vector2 checkingPoint;
    public bool wander = false;
    [SerializeField] private bool completedPath = true;
    private float maxWanderTime;
    private float minWanderTime;
    private float wanderLimit;


    [Header("General Enemy Info")]
    [SerializeField] private float speed;
    [SerializeField] private float huntSpeed;
    [SerializeField] private float roamSpeed;
    private Rigidbody2D rb;
    public int shotsTaken;

    [Header("Other Game Objects")]
    public GameObject player;
    public GameObject tempGameObject;
    private WeaponScript weaponScript;
    private EnemyWeaponsScript enemyWeaponScript;
    private GameObject centralDataHub;
    private GameObject AllNodes;
    public Animator legAnimator;

    [Header("Miscellaneous")]
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    private SoundDetectionScript soundDetection;
    private OtherData otherData;
    private List<GameObject> pathPointsList = new List<GameObject>();
    private List<GameObject> searchNodesList = new List<GameObject>();
    [SerializeField] private int listTracker = 0;
    [SerializeField] private bool isLookingAtSide = false;
    private AnimationCurve rotationCurve;
    [SerializeField] private float rotateSpeed;
    //Distance from player in which the players exact position can be obtained. Else, any way the player is detected will result in the rooms within this distance
    //away from the player being searched, but not the exact position.
    private float playerExactPosDist;
    public bool rotate = false;
    private Quaternion targetRotation;
    private Vector2 locationTrackerPosition;
    private float changeDist;

    private void Awake()
    {
        centralDataHub = GameObject.FindGameObjectWithTag("CentralDataHub");
        otherData = centralDataHub.GetComponent<OtherData>();
        roamSpeed = CentralDataScript.enemyRoamSpeed;
        speed = roamSpeed;
        huntSpeed = CentralDataScript.enemyHuntSpeed;
        aiBehaviour = otherData.behaviours[UnityEngine.Random.Range(0, otherData.behaviours.Length)];
        SetBehaviour();
        targetMask = otherData.targetMask;
        obstructionMask = otherData.obstructionMask;
        playerExactPosDist = CentralDataScript.exactPlayerPosLimit;
        rotationCurve = otherData.rotationCurve;
        rotateSpeed = CentralDataScript.rotateSpeed;
        radius = CentralDataScript.radius;
        angle = CentralDataScript.angle;
        maxWanderTime = CentralDataScript.maxWanderTime;
        minWanderTime = CentralDataScript.minWanderTime;
        wanderLimit = CentralDataScript.wanderLimit;
        changeDist = CentralDataScript.changeDist;

        player = GameObject.FindGameObjectWithTag("Player");
        AllNodes = GameObject.FindGameObjectWithTag("AllNodes");
        weaponScript = player.GetComponent<WeaponScript>();
        enemyWeaponScript = GetComponent<EnemyWeaponsScript>();
        soundDetection = GetComponent<SoundDetectionScript>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(FOVRoutine());
        InvokeRepeating("GeneratePath", 0f, .5f);
        Invoke("enemySpriteManager", 0.1f);
        aiState = "IdlePath";
    }

    private void Update()
    {
        if (target != null)
            Debug.Log("Target:" + target.name);
        EnemyBehaviour();
        Debug.Log("stop movin? U" + stopMoving);
    }

    private void FixedUpdate()
    {
        StateManager();

        tempGameObject.transform.position = locationTrackerPosition;
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

    private void GeneratePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
        }
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        { 

            yield return wait;
            FOVCheck();
        }
    }

    private void LookToTarget(GameObject target)
    {
        var difference = target.transform.position - transform.position; 
        var angle = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.back);
        RotateSmoothly(targetRotation);
    }

    private void LookToTarget(Vector2 point)
    {
        var difference = point - new Vector2(transform.position.x, transform.position.y);
        var angle = Mathf.Atan2(difference.x, difference.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.back);
        RotateSmoothly(targetRotation);
    }

    private void RotateSmoothly(Quaternion rotation)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationCurve.Evaluate(rotateSpeed * Time.deltaTime));
    }

    private void EnemyMove(Vector2 directionToObj)
    {
        if (!stopMoving)
        {
            Debug.Log("im MOVIN");
            rb.velocity = directionToObj * speed * Time.deltaTime;
            legAnimator.SetBool("isMoving", true);
        }

        else
        {
            rb.velocity = Vector2.zero;
            legAnimator.SetBool("isMoving", false);
        }   
    }


    private void EnemyBehaviour()
    { 
        if (target != null)
            distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (currentPoint >= patrolPoints.Length)
            currentPoint = 0;          

        if (!canSeePlayer && !hasDetectedPlayer)
        {
            if (wander)
                aiState = "Wander";
            else
                aiState = "IdlePath";
        }

        else if (canSeePlayer)
        {
            aiState = "ChasePlayer";
        }

        else
        {
            aiState = "SearchLastKnown";
        }
    }

    private void SetBehaviour()
    {
        if (aiBehaviour == "aggressive")
        {
            maxDistanceToTarget = CentralDataScript.aggresiveDistToTarget;
        }

        else if (aiBehaviour == "cautious")
        {
            maxDistanceToTarget = CentralDataScript.cautiousDistToTarget;
        }
    }

    private void StateManager()
    {
        Debug.Log("state:" + aiState);
        switch (aiState)
        {
            case "IdlePath":
                PatrolSetPath();
                break;

            case "Wander":
                WanderBehaviour();
                break;

            case "ChasePlayer":
                ChasePlayer();
                break;

            case "SearchLastKnown":
                SearchLastKnown();
                break;
        }


        MoveAI();
        if (!rotate)
            if (target != null)
            {
                LookToTarget(target);
            }
                    
        if (rotate)
        {
            RotateSmoothly(targetRotation);
            if (transform.rotation == targetRotation)
                rotate = false;
        }
    }

    private void PatrolSetPath()
    {
        target = patrolPoints[currentPoint].gameObject;
        if (distanceToTarget <= 1)
            currentPoint++;
    }

    private void SearchLastKnown()
    {
        speed = huntSpeed;
        Invoke("DelayedLastPlayerLocation", 1); //Sets lastPlayerLocation after one second so the AI can know roughly where the player went after they went out of sight.
        locationTrackerPosition = lastPlayerLocation;
        target = tempGameObject;
        float distToLPL = Vector2.Distance(lastPlayerLocation, transform.position); //Means 'distance to last player location'
        if (distToLPL <= changeDist)
        {
            if (!isChecking)
            {
                StartCoroutine(RotateRandomly(1));
            }
        }
    }

    private void WanderBehaviour()
    {
        speed = roamSpeed;
        if (!isChecking)
        {
            if (completedPath)
            {
                int randomNum = UnityEngine.Random.Range(1, 8);
                Debug.Log("Random num: " + randomNum);
                if (randomNum == 1)
                {
                    float randomTime = UnityEngine.Random.Range(1, 2);
                    StartCoroutine(RotateRandomly(randomTime));
                }
                else
                {
                    float randomXPos = UnityEngine.Random.Range(transform.position.x - wanderLimit, transform.position.x + wanderLimit);
                    float randomYPos = UnityEngine.Random.Range(transform.position.y - wanderLimit, transform.position.y + wanderLimit);
                    locationTrackerPosition = new Vector2(randomXPos, randomYPos);
                    Debug.Log("pos:" + locationTrackerPosition.ToString());
                    Vector3 vec3Pos = locationTrackerPosition;
                    Vector2 directionToPos = (vec3Pos - transform.position).normalized;
                    float distToPoint = Vector2.Distance(transform.position, locationTrackerPosition);
                    RaycastHit2D checkRay = Physics2D.Raycast(transform.position, directionToPos, distToPoint, obstructionMask);
                    if (checkRay.point != new Vector2(0, 0))
                    {
                        locationTrackerPosition = checkRay.point;
                        Debug.DrawLine(transform.position, checkRay.point, Color.red, 10);
                    }
                    else
                        Debug.DrawLine(transform.position, locationTrackerPosition, Color.blue, 10);
                    target = tempGameObject;
                    completedPath = false;
                }
            }

            else
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);
                //Debug.Log("Distance:" + distance);
                if (distance < changeDist)
                {
                    completedPath = true;
                    rb.velocity = Vector2.zero;
                }
            }
        }
    } 

    private IEnumerator RotateRandomly(float timeBetween)
    {
        isChecking = true;
        rotate = true;
        stopMoving = true;
        Debug.Log("stop movin? 2" + stopMoving);
        int randomInt = UnityEngine.Random.Range(3, 5);
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        for (int i = 0; i <= randomInt; i++)
        {
            Debug.Log("fr rotate randomly");
            if (canSeePlayer)
            {
                StopRandomRot();
                yield break;
            }
                
            yield return new WaitForSeconds(timeBetween);
            float randomZRot = UnityEngine.Random.Range(-180, 180);
            targetRotation = Quaternion.Euler(0, 0, randomZRot);
        }
        StopRandomRot();
    }
    
    private void StopRandomRot()
    {
        rotate = false;
        soundDetection.detectedPlayer = false;
        isChecking = false;
        stopMoving = false;
    }

    private void ChasePlayer()
    {
        speed = huntSpeed;
        if (!hasDetectedPlayer)
            enemyWeaponScript.ReactionTime(true);
        enemyWeaponScript.ReactionTime(false);
        lastPlayerLocation = player.transform.position;
        tempGameObject.transform.position = lastPlayerLocation; 
        target = player;
        soundDetection.detectedPlayer = true;
    }

    public void ResetVariables()
    {
        isChecking = false;
        rotate = false;
        stopMoving = false;
    }

    private void FOVCheck()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        if(rangeChecks.Length != 0)
        {
            Transform targetFOV = rangeChecks[0].transform;
            Vector2 directionToTarget = (targetFOV.position - transform.position).normalized;

            if (Vector2.Angle(transform.up, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, targetFOV.position);

                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    if (isChecking)
                        isChecking = false;

                    if (rotate)
                        rotate = false;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }

        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }

    void MoveAI()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }

        else
        {
            reachedEndOfPath = false;
        }

        float playerDistance = Vector2.Distance(transform.position, player.transform.position);
        if (canSeePlayer && playerDistance <= maxDistanceToTarget)
        {
            stopMoving = true;
        }

        else if (isChecking == false)
        {
            stopMoving = false;
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance <= nextWaypointDist && path.vectorPath[currentWaypoint] != path.vectorPath.Last())
        {
            currentWaypoint++;
        }

        else
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            EnemyMove(direction);
        }

    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
