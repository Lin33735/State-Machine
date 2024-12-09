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
    Material mat;
    Color color;
    public GameObject winPanel;

    [Header("Movement")]
    [SerializeField] private float baseSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private bool right;
    [SerializeField] private Vector3 circle;

    [Header("HitPoints")]
    [SerializeField] private GameObject ChargeATKHitBox;
    [SerializeField] private int MaxHP = 100;
    [SerializeField] private int curHP;
    public StatBars BossHPBar;

    [Header("Target")]
    [SerializeField] private GameObject Target;
    [SerializeField] private float distanceFromTarget;
    [SerializeField] private Vector3 direction;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Quaternion targetRotation;

    [Header("Attack Settings")]
    [SerializeField] private GameObject Projectile;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float shootTimer;
    [SerializeField] private float shootDur;
    [SerializeField] private float IdleStateDur;

    [Header("States")]
    [SerializeField] private bool trySwitchState;
    public BossState curState;

    public enum BossState
    {
        Idle,
        Charge,
        Shoot,
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;
    }

    void Start()
    {
        color = mat.color; 

        curHP = MaxHP;
        BossHPBar.MaxBossHP(MaxHP);
        speed = baseSpeed;

        ChangeState(BossState.Idle);
    }

    private void Update()
    {
        distanceFromTarget = Vector3.Distance(transform.position, Target.transform.position);

        if (curHP < 0)
        {
            Defeated();
        }
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
                break;
            case BossState.Charge:
                speed = baseSpeed;
                LookAt();
                targetPosition = Target.transform.position;
                ChargeATKHitBox.gameObject.SetActive(true);
                break;
            case BossState.Shoot:
                rb.useGravity = false;
                shootSpeed = 0.1f;
                shootDur = 10;
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
                IdleStateDur += Time.deltaTime;
                if (IdleStateDur > 4)
                {
                    IdleStateDur = 0;
                    StartCoroutine(AttemptSwitchState());
                }
                break;
            case BossState.Charge:
                ChargeAtTarget();
                break;
            case BossState.Shoot:
                LookAt();
                if (transform.position.y < 10)
                {
                    Levitate();
                }
                else
                {
                    ShootAtTarget();
                }
                if (distanceFromTarget > 60)
                {
                    StartCoroutine(Stun());
                }
                break;
        }
    }

    void OnExitState(BossState exitState)
    {
        switch (exitState)
        {
            case BossState.Idle:
                break;
            case BossState.Charge:
                ChargeATKHitBox.gameObject.SetActive(false);
                break;
            case BossState.Shoot:
                rb.useGravity = true;
                break;
        }
    }

    void ChangeState(BossState newState)
    {
        OnExitState(curState);
        curState = newState;
        OnEnterState(curState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerAttackHitBox")
        {
            StartCoroutine(TakeDamage());
        }
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
        rb.MovePosition(this.transform.position + speed * Time.fixedDeltaTime * direction.normalized);
    }

    void ChargeAtTarget()
    {
        speed = 150f;
        ///MoveInDirection(direction, speed);

        Vector3 targetPositionXZ = new Vector3(targetPosition.x, (this).transform.position.y, targetPosition.z);
        transform.position = Vector3.MoveTowards(this.transform.position, targetPositionXZ, speed * Time.deltaTime);

        if (targetPositionXZ == this.transform.position)
        {
            ChangeState(BossState.Idle);
        }
    }

    void ShootAtTarget()
    {
        shootTimer += Time.deltaTime;
        shootDur -= Time.deltaTime;

        if (shootTimer > shootSpeed)
        {
            Instantiate(Projectile,this.transform.position, Quaternion.identity);
            shootTimer = 0;
        }
        if (shootDur < 0)
        {
            StartCoroutine(Stun());
        }
    }

    void Levitate()
    {
        Vector3 levpos = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        transform.position = Vector3.MoveTowards(this.transform.position, levpos, speed * Time.deltaTime);
    }

    void Defeated()
    {
        gameObject.SetActive(false);
        winPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    private IEnumerator TakeDamage()
    {
        curHP -= 20;
        BossHPBar.SetBossHP(curHP);
        mat.color = Color.white;
        yield return new WaitForSeconds(0.3f);
        mat.color = color;
    }

    private IEnumerator AttemptSwitchState()
    {
        if (Random.Range(0, 3) == 0)
        {
            ChangeState(BossState.Shoot);
        }
        else
        {
            right = Random.Range(0, 2) == 0;
        }
        yield return this;
    }

    private IEnumerator Stun()
    {
        yield return new WaitForSeconds(2);
        ChangeState(BossState.Idle);
    }
}
