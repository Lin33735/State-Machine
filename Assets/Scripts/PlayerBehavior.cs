using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static BossBehavior;

public class PlayerBehavior : MonoBehaviour
{
    public bool Debugs = true;

    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 10;
    [SerializeField] private float speed;
    [SerializeField] private float curSpeed;

    [Header("Dashing Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDur;
    [SerializeField] private bool canDash;
    [SerializeField] private float dashCD;

    [Header("Attack Settings")]
    [SerializeField] private GameObject AttackHitBox;
    [SerializeField] private float attackCharge;
    [SerializeField] private float attackChargeSpeed;
    [SerializeField] private bool attackCharged;

    public StatBars StaminaBar;
    public StatBars ATKChargeBar;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        canDash = true;
        dashSpeed = 100;
        dashDur = 0.2f;
        dashCD = 0;


        attackChargeSpeed = 2;

        speed = baseSpeed;

        ChangeState(PlayerState.Grounded);
    }

    public enum PlayerState
    {
        Grounded,
        Attack,
    }
    public PlayerState curState;

    void OnEnterState(PlayerState enterState)
    {
        switch (enterState)
        {
            case PlayerState.Grounded:
                speed = baseSpeed;
                break;
            case PlayerState.Attack:
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
            case PlayerState.Attack:
                Walk();
                ChargingAttack();
                break;
        }
    }

    void OnExitState(PlayerState exitState)
    {
        switch (exitState)
        {
            case PlayerState.Grounded:
                break;
            case PlayerState.Attack:
                speed = baseSpeed;
                attackCharge = 0;
                ATKChargeBar.SetATKCharge(attackCharge);
                if (attackCharged)
                {
                    attackCharged = false;
                    AttackHitBox.gameObject.SetActive(false);
                }
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
        OnInputDash();
        OnHoldAttackState();
    }

    private void FixedUpdate()
    {
        FixedUpdateState(curState);

        if (dashCD >= 0)
        {
            dashCD -= Time.deltaTime;
            StaminaBar.SetStamina(dashCD);
            if (dashCD < 0)
            {
                canDash = true;
            }
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

    void OnHoldAttackState()
    {
        var chargeATK = Input.GetMouseButtonDown(0);
        var releaseATK = Input.GetMouseButtonUp(0);

        if (chargeATK)
        {
            ChangeState(PlayerState.Attack);
        }
        if (releaseATK && attackCharged)
        {
            StartCoroutine(Attack());
        }
        if (releaseATK && !attackCharged)
        {
            ChangeState(PlayerState.Grounded);
        }
    }

    void OnInputDash()
    {
        var dashInput = Input.GetButtonDown("Dash");

        if (dashInput && canDash)
        {
            curSpeed = speed;
            speed = dashSpeed;
            canDash = false;
            StartCoroutine(StopDashing());
        }
    }

    void ChargingAttack()
    {
        if (attackCharge > attackChargeSpeed)
        {
            attackCharged = true;
        }
        else
        {
            attackCharge += Time.deltaTime;
            ATKChargeBar.SetATKCharge(attackCharge);
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashDur);
        speed = curSpeed;
        dashCD = 2;
        StaminaBar.MaxStamina(dashCD);
    }

    private IEnumerator Attack()
    {
        AttackHitBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ChangeState(PlayerState.Grounded);
    }
}
