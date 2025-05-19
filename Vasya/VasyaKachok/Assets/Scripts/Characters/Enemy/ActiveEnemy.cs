
using UnityEngine.AI;
using UnityEngine;
using Unity.VisualScripting;

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    Dead
}

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyAnimatorController))]
public class EnemyController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float extraRotationSpeed = 100f;

    private Transform target;
    private NavMeshAgent agent;
    private EnemyAnimatorController animatorController;
    private EnemyState currentState = EnemyState.Idle;
    private float currentHealth;
    private float lastAttackTime;


    public bool FoundPlayer = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorController = GetComponent<EnemyAnimatorController>();
        currentHealth = enemyData.health;

        agent.speed = enemyData.moveSpeed;
        agent.stoppingDistance = attackRange;
    }

    private void Update()
    {
        if (currentState == EnemyState.Dead) return;

        if (FoundPlayer) 
        {
            Activate();
            FoundPlayer = false;
        }

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            Vector3 lookrotation = target.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);

            if (distanceToTarget <= attackRange && currentState != EnemyState.Attack)
            {
                StartAttack();

            }
            else if (distanceToTarget > attackRange && currentState != EnemyState.Chase)
            {
                StartChase();
            }

            if (currentState == EnemyState.Chase)
            {
                agent.SetDestination(target.position);
                    animatorController.ChangeAnimation("Standard Run");

            }
        }
        else
        {
            currentState = EnemyState.Idle;
            animatorController.ChangeAnimation("Idle");
        }    
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void Activate()
    {
        if (target == null)
        {
            // Поиск игрока если цель не задана
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        StartChase();
    }

    public void TakeDamage(int damage)
    {
        if (currentState == EnemyState.Dead) return;

        currentHealth -= damage;
        animatorController.ChangeAnimation("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void StartChase()
    {
        currentState = EnemyState.Chase;
        agent.isStopped = false;


    }

    private void StartAttack()
    {

        animatorController.ChangeAnimation("Cross Punch");
        currentState = EnemyState.Attack;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        lastAttackTime = Time.time;
    }

    private void CompleteAttack()
    {
        if (target.TryGetComponent<ActivePlayer>(out var damageable))
        {
            damageable.TakeDamage(enemyData.baseDamage);
        }
    }

    private void Die()
    {
        currentState = EnemyState.Dead;
        agent.isStopped = true;
        animatorController.ChangeAnimation("Two Handed Sword Death");
        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}