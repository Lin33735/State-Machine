using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public bool Debugs = true;

    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed;
    private float speed;

    [Header("Dashing Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDur;
    [SerializeField] private bool canDash;

    Rigidbody rb;

    public enum PlayerState
    {
        Grounded,
        Dashing,
        ChargingATK,
    }
    public PlayerState state;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = baseSpeed;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Grounded:
                Walk();
                break;

            case PlayerState.Dashing:
                Dash();
                break;
        }
    }

    Vector3 Dir(bool Debugs)
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 curDir = new Vector3(x, 0, y);


        if (Debugs)
        {
            Debug.DrawRay(transform.position, rb.velocity, Color.yellow);
            //Debug.Log("Vector: " + curDir);
            Debug.DrawRay(transform.position, transform.TransformDirection(curDir) * 2f, Color.yellow);
        }
        return curDir;
    }

    void Walk()
    {
        Vector3 aimDir = (transform.TransformDirection(Dir(Debugs)));
        rb.velocity = new Vector3(aimDir.x * speed, rb.velocity.y, aimDir.z * speed);
    }

    void Dash()
    {

    }

}
