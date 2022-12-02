using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    public EnemyAI enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<EnemyAI>();
    }

    void Update()
    {
        if (!enemy.playerInTrigger)
        {
            enemy.timeNotInCollider += Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && enemy != null)
        {
            enemy.timeInCollider = 0f;
            if (enemy.timeNotInCollider > 0.3f)
            {
                enemy.timeNotInCollider = 0f;
                enemy.DamagePlayer();
            }
            enemy.playerInTrigger = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && enemy != null)
        {
            if (enemy.timeInCollider < 0.3)
            {
                enemy.timeInCollider += Time.deltaTime;
            }
            else
            {
                enemy.DamagePlayer();
                enemy.timeInCollider = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && enemy != null)
        {
            enemy.playerInTrigger = false;
        }
    }
}
