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
        var deadState = fsm.CreateState("Dead");

        patrolingState.onEnter = delegate
        {
            
        };

        patrolingState.onFrame = delegate
        {
            
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
    }

    

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }

    public void DamagePlayer()
    {
        //player.GetComponent<PlayerController>().TakeDamage(damage);
        Debug.Log("Damaged Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touch");
    }
   
}
