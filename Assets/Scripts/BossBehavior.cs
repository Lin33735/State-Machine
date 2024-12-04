using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

public class BossBehavior : MonoBehaviour
{
    public bool debug = true;
    Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float baseSpeed;
    private float speed;
    public float RotationSpeed;
    [SerializeField] private bool right;
    [SerializeField] private Vector3 circle;

    [Header("HitPoints")]
    [SerializeField] private int MaxHP = 100;
    public int curHP;
    public UnityEvent<int> Damage;

    [Header("Target")]
    public GameObject Target;
    [SerializeField] private float distanceFromTarget;
    [SerializeField] private Vector3 direction;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    [Header("States")]
    public BossState curState; 
    public enum BossState
    {
        Idle,
        Charge,
        Leap,
        Throw,
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        curHP = MaxHP;
        speed = baseSpeed;

        ChangeState(BossState.Idle);
    }
    private void Update()
    {
        distanceFromTarget = Vector3.Distance(transform.position, Target.transform.position);
    }
    private void FixedUpdate()
    {
        FixedUpdateState(curState);
    }
    
    void OnEnterState(BossState enterState)
    {
        switch (enterState)
        {
            case BossState.Idle:
                speed = baseSpeed / 2;
                right = Random.Range(0, 2) == 0;
                StartCoroutine(IdleDur());
                break;
            case BossState.Charge:
                speed = baseSpeed;
                LookAt();
                targetPosition = Target.transform.position;
                break;
        }
    }

    void FixedUpdateState(BossState fUpdateState)
    {
        switch (fUpdateState)
        {
            case BossState.Idle:
                LookAt();
                if (distanceFromTarget > 30)
                {
                    Vector3 combinedMovement = DirectionMove() + CirclePlayer();
                    MoveInDirection(combinedMovement, speed);
                }
                else
                {
                    MoveInDirection(CirclePlayer(), speed/3);
                }
                if (distanceFromTarget > 40)
                {
                    ChangeState(BossState.Charge);
                }
                break;
            case BossState.Charge:
                StartCoroutine(Charge());
                break;
        }

    }

    void OnExitState(BossState exitState)
    {
        switch (exitState)
        {
            case BossState.Idle:
                break;
        }
    }

    void ChangeState(BossState newState)
    {
        OnExitState(curState);
        curState = newState;
        OnEnterState(curState);

    }

    void LookAt()
    {
        direction = Target.transform.position - transform.position;
        
        float angle = Mathf.Atan2(direction.x, direction.z) *Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(0f, angle, 0f);
        
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    Vector3 DirectionMove()
    {
        return direction.normalized;
    }

    Vector3 CirclePlayer()
    {
        if (right)
        {
            return transform.TransformDirection(Vector3.right);
        }
        else
        {
            return transform.TransformDirection(Vector3.left);
        }
    }

    void MoveInDirection(Vector3 direction, float speed)
    {
        rb.MovePosition(this.transform.position + speed * Time.deltaTime * direction.normalized);
    }

    void ChargeAtTarget()
    {
        speed = 115f;

        Vector3 targetPositionXZ = new Vector3(targetPosition.x, (this).transform.position.y, targetPosition.z);

        transform.position = Vector3.MoveTowards(this.transform.position, targetPositionXZ, speed * Time.deltaTime);
    }

    private IEnumerator Charge()
    {
        yield return new WaitForSeconds(1);
        ChargeAtTarget();
        yield return new WaitForSeconds(1.5f);
        ChargeAtTarget();
        ChangeState(BossState.Idle);
    }
    private IEnumerator IdleDur()
    {
        yield return new WaitForSeconds(3);
        if (Random.Range(0 ,6) == 0)
        {
            ChangeState(BossState.Throw);
        }
        else
        {
            ChangeState(BossState.Idle);
        }
    }
}