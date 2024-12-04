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
    [SerializeField] private float speed;

    [Header("Dashing Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDur;
    [SerializeField] private bool canDash;
    [SerializeField] private float dashCD;

    Rigidbody rb;

    public enum PlayerState
    {
        Grounded,
        ChargingATK,
    }
    public PlayerState state;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = baseSpeed;
        canDash = true;
    }

    private void Update()
    {
        var dashInput = Input.GetButtonDown("Dash");
        
        if (dashInput && canDash)
        {
            Dash();
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.Grounded:
                Walk();
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
        speed = dashSpeed;
        canDash = false;
        StartCoroutine(StopDashing());
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashDur);
        speed = baseSpeed;
        StartCoroutine (DashCoolDown());
        Debug.Log("Dash End");
    }

    private IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }
}
