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

    public LayerMask whatIsGround, whatIsPlayer;

    [Header("FOV")]
    public float radius;
    [Range(0, 360)]
    public float angle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;

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

    [Header("Knockback")]
    public bool isknockedBack = false;
    private Vector3 knockbackDirection;

    [Header("Ranges/States")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, walkCoroutineRunning;

    private void Awake()
    {
        player = GameObject.Find("Player").transform; //find the player in the scene
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        startingPoint = transform.position;
    }

    private void Start()
    {
        StartCoroutine(FOVRoutine());

        fsm = new FiniteStateMachine();
        var patrolingState = fsm.CreateState("Patroling");
        var ChasingState = fsm.CreateState("Chasing");
        var deadState = fsm.CreateState("Dead");
        var knockbackState = fsm.CreateState("Knockback");

        patrolingState.onEnter = delegate
        {
            
        };

        patrolingState.onFrame = delegate
        {
            if (walkPointSet)
            {
                if (!walkCoroutineRunning)
                {
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
                StopCoroutine(WalkToPoint());
                walkPointSet = false;
            }

            if (canSeePlayer)
            {
                fsm.TransitionTo("Chasing");
            }
        };

        patrolingState.onExit = delegate
        {
            StopCoroutine(WalkToPoint());
        };

        //chasing state
        ChasingState.onEnter = delegate
        {
            
        };

        ChasingState.onFrame = delegate
        {
            if (agent != null)
            {
                agent.SetDestination(player.position);
            }
            
            if (!canSeePlayer && !playerInAttackRange)
            {
                fsm.TransitionTo("Patroling");
            }
        };

        ChasingState.onExit = delegate
        {
           
        };

        deadState.onEnter = delegate
        {
            
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
                    canSeePlayer = true;
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
        yield return new WaitForSeconds(3);
        agent.SetDestination(walkPoint);
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
        player.GetComponent<PlayerController>().TakeDamage(damage);
        OnKnockback(-transform.forward * 0.2f);
        Debug.Log("Damaghed Player");
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
