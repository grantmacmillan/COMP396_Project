using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [Header("Unity")]
    public NavMeshAgent agent;
    public Transform player;
    public Vector3 startingPoint;
    FiniteStateMachine fsm;
    public Rigidbody rb;
    private Animator anim;
    public LayerMask whatIsGround, whatIsPlayer;
    

    [Header("FOV")]
    public float radius;
    [Range(0, 360)]
    public float angle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;
    public float sightTimer;
    public float maxTimeWithoutSight;

    [Header("Attacking")]
    public bool playerInTrigger = false;
    public float damage;
    public float timeInCollider;
    public float timeNotInCollider;

    [Header("Health")]
    public float health;
    public float maxHealth;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float walkTimer;
    public GameObject footSteps;

    [Header("Knockback")]
    public bool isknockedBack = false;
    private Vector3 knockbackDirection;

    [Header("Ranges/States")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, walkCoroutineRunning;

    private void Awake()
    {
        footSteps.SetActive(false);
        player = GameObject.Find("Player").transform; //find the player in the scene
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        startingPoint = transform.position;
        anim = GetComponentInChildren<Animator>();
    }
    int i = 0;
    private void Start()
    {
        maxHealth = 20;
        health = maxHealth;
        StartCoroutine(FOVRoutine());

        fsm = new FiniteStateMachine();
        var patrolingState = fsm.CreateState("Patroling");
        var ChasingState = fsm.CreateState("Chasing");
        var deadState = fsm.CreateState("Dead");
        var knockbackState = fsm.CreateState("Knockback");

        patrolingState.onEnter = delegate
        {
            agent.speed = 3f;
            anim.SetBool("isRunning", false);
            
        };

        patrolingState.onFrame = delegate
        {
            if (walkPointSet)
            {
                if (!walkCoroutineRunning)
                {
                    
                    if(i != 0)
                    {
                        footSteps.SetActive(true);
                        anim.SetBool("isWalking", true);
                    }
                    StartCoroutine(WalkToPoint());
                }
                

            }
            if (!walkPointSet)
            {
                
                walkPoint = RandomNavSphere(startingPoint, walkPointRange, -1);
                walkPointSet = true;
            }
            
            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //walkpoint reached
            if (distanceToWalkPoint.magnitude < 0.1f)
            {
                anim.SetBool("isWalking", false);
                footSteps.SetActive(false);
                StopCoroutine(WalkToPoint());
                walkPointSet = false;
            }

            if (canSeePlayer)
            {
                anim.SetBool("isRunning", true);
                footSteps.SetActive(true);
                anim.SetBool("isWalking", false);
                fsm.TransitionTo("Chasing");
            }
        };

        patrolingState.onExit = delegate
        {
            anim.SetBool("isWalking", false);
            footSteps.SetActive(false);
            StopCoroutine(WalkToPoint());
        };

        //chasing state
        ChasingState.onEnter = delegate
        {
            footSteps.SetActive(true);
            agent.speed = 7f;
        };

        ChasingState.onFrame = delegate
        {
            if (agent != null)
            {
                agent.SetDestination(player.position);
            }

            sightTimer += Time.deltaTime;
            if (sightTimer > maxTimeWithoutSight)
            {
                if (!canSeePlayer && !playerInAttackRange)
                {
                    fsm.TransitionTo("Patroling");
                }
            }
            
            
        };

        ChasingState.onExit = delegate
        {
            anim.SetBool("isRunning", false);
            footSteps.SetActive(false);
        };

        deadState.onEnter = delegate
        {
            Destroy(gameObject);
        };

        deadState.onFrame = delegate
        {
            //put death code here
            
        };

        deadState.onExit = delegate
        {

        };

        knockbackState.onEnter = delegate
        {
            StartCoroutine(Knockback());
        };

        knockbackState.onFrame = delegate
        {
            if (!isknockedBack)
            {
                fsm.TransitionTo("Chasing");
            }
        };

        knockbackState.onExit = delegate
        {

        };
    }

    

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
        //check if player is in range of vision or attack range (or neither)
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position + new Vector3(0,1.5f,0), radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    sightTimer = 0;
                }
                    
                else if (!Physics.Raycast(transform.position + new Vector3(0, 3f, 0), directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    sightTimer = 0;
                }
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }


    IEnumerator WalkToPoint()
    {
        walkCoroutineRunning = true;
        yield return new WaitForSeconds(walkTimer);
        agent.SetDestination(walkPoint);
        i++;
        walkCoroutineRunning = false;
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }


    public void DamagePlayer()
    {
        //player.GetComponent<PlayerController>().TakeDamage(damage);
        //OnKnockback(-transform.forward * 0.2f);
        player.GetComponent<PlayerController>().PlayerDeath();   
    }

    public void TakeDamage(float damage, Vector3 force)
    {
        health -= damage;

        if (health <= 0)
        {
            fsm.TransitionTo("Dead");
        }
        else
        {
            OnKnockback(force);
        }

    }

    IEnumerator Knockback()
    {
        if (rb != null)
        {
            isknockedBack = true;
            agent.enabled = false;
            rb.AddForce(knockbackDirection, ForceMode.Impulse);
            yield return new WaitForSeconds(0.1f);
        }
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            agent.enabled = true;
            isknockedBack = false;
        }
    }

    public void OnKnockback(Vector3 directionForce)
    {
        knockbackDirection = directionForce;
        fsm.TransitionTo("Knockback");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touch");
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startingPoint, walkPointRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
