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
    FiniteStateMachine fsm;
    public Rigidbody rb;

    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Attacking")]
    public bool playerInTrigger = false;
    public float damage;

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
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform; //find the player in the scene
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        fsm = new FiniteStateMachine();
        var patrolingState = fsm.CreateState("Patroling");
        var ChasingState = fsm.CreateState("Chasing");
        var attackingState = fsm.CreateState("Attacking");
        var deadState = fsm.CreateState("Dead");
        var knockbackState = fsm.CreateState("Knockback");

        patrolingState.onEnter = delegate
        {
            
        };

        patrolingState.onFrame = delegate
        {
            fsm.TransitionTo("Chasing");
        };

        patrolingState.onExit = delegate
        {
           
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
        };

        ChasingState.onExit = delegate
        {
           
        };

        attackingState.onEnter = delegate
        {

        };

        attackingState.onFrame = delegate
        {
            fsm.TransitionTo("Chasing");
        };

        attackingState.onExit = delegate
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

    public void DamagePlayer()
    {
        //player.GetComponent<PlayerController>().TakeDamage(damage);
        Debug.Log("Damaged Player");
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
   
}
