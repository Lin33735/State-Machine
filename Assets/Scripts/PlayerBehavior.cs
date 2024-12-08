using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using static BossBehavior;

public class PlayerBehavior : MonoBehaviour
{
    public bool Debugs = true;

    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 10;
    [SerializeField] private float speed;

    [Header("Dashing Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDur;
    [SerializeField] private bool canDash;
    [SerializeField] private float dashCD;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        canDash = true;

        ChangeState(PlayerState.Grounded);
    }

    public enum PlayerState
    {
        Grounded,
        ChargingATK,
    }
    public PlayerState curState;

    void OnEnterState(PlayerState enterState)
    {
        switch (enterState)
        {
            case PlayerState.Grounded:
                speed = baseSpeed;
                break;
            case PlayerState.ChargingATK:
                speed = 1;  
                break;
        }
    }
    void FixedUpdateState(PlayerState fUpdateState)
    {
        switch (fUpdateState)
        {
            case PlayerState.Grounded:
                Walk();
                break;
            case PlayerState.ChargingATK:
                Walk();
                Debug.Log("ChargingAttack");
                break;
        }
    }
    void OnExitState(PlayerState exitState)
    {
        switch (exitState)
        {
            case PlayerState.Grounded:
                break;
            case PlayerState.ChargingATK:
                break;
        }
    }

    void ChangeState(PlayerState newState)
    {
        if (curState == newState)
        {
            return;
        }

        OnExitState(curState);
        curState = newState;
        OnEnterState(curState);
    }

    private void Update()
    {
        var dashInput = Input.GetButtonDown("Dash");
        
        if (dashInput && canDash)
        {
            Dash();
        }

        var chargeATK = Input.GetMouseButtonDown(0);

        if (chargeATK)
        {
            ChangeState(PlayerState.ChargingATK);
        }
        else
        {
            ChangeState(PlayerState.Grounded);
        }
    }

    private void FixedUpdate()
    {
        FixedUpdateState(curState);
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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "EnemyHitBox")
        {
            Debug.Log("Player Hit");
            this.gameObject.SetActive(false);
        }
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
