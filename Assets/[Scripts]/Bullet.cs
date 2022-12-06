using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject impactEffect;
    public LayerMask enemiesMask;

    [Header("Bullet Stats")]
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;
    public float damage;
    public float range;
    public float knockback;
    public int maxCollisions;
    public float maxLifetime;
    public bool destroyOnImpact = true;
    public bool isBullet = true;

    private int collisions;
    private PhysicMaterial material;
    private Vector3 prev = Vector3.zero;

    private void Start()
    {
        Setup();
    }
    
    private void Update()
    {
        if(collisions >= maxCollisions) 
            Impact(null);

        maxLifetime -= Time.deltaTime;

        if (maxLifetime <= 0)
            Impact(null);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;

        if (collision.collider.CompareTag("Enemy") && destroyOnImpact)
        {
            Impact(collision.collider);
            //Debug.Log("Bullet Collision");
        }

    }

    private void OnTriggerEnter(Collider collider)
    {
        collisions++;
        if (collider.CompareTag("Enemy") && destroyOnImpact)
        {
            Impact(collider);
            Debug.Log("Bullet Trigger");
        }
            
    }
    private void Impact(Collider collider)
    {
        
        
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        if (isBullet && collider != null)
        {
            EnemyAI enemy = collider.GetComponentInParent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, transform.forward * knockback);
                FindObjectOfType<SoundManager>().Play("Hitmarker");
            }
            
        }
        else if(!isBullet)
        {
            //FindObjectOfType<SoundManager>().Play("Hitmarker");
            Collider[] enemies = Physics.OverlapSphere(transform.position, range, enemiesMask, QueryTriggerInteraction.Collide);

            foreach (Collider enemy in enemies)
            {
                enemy.GetComponentInParent<EnemyAI>().TakeDamage(damage, transform.forward * knockback);
            }
        }
        Invoke("Destroy", 0.01f);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    private void Setup()
    {
        material = new PhysicMaterial();
        material.bounciness = bounciness;
        material.frictionCombine = PhysicMaterialCombine.Minimum;
        material.bounceCombine = PhysicMaterialCombine.Maximum;
        GetComponent<Collider>().material = material;

        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
