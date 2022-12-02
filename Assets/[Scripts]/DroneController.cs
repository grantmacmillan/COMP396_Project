using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 moveDir;
    public Transform orientation;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        SpeedControl();
    }

    public void Move(Vector2 input)
    {
        moveDir = Vector3.zero;
        moveDir = orientation.forward * input.y + orientation.right * input.x;
        rb.AddForce(moveDir.normalized * speed * 25f, ForceMode.Force);
    }

    public void MoveUpDown(Vector2 input)
    {
        rb.AddForce(Vector3.up * input.y * speed * 10f, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (vel.magnitude > speed)
        {
            Vector3 controlVel = vel.normalized * speed;

            rb.velocity = new Vector3(controlVel.x, rb.velocity.y, controlVel.z);
        }
    }
}
