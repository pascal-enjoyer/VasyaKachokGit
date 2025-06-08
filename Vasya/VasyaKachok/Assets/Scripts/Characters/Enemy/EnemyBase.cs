using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(AnimationController), typeof(EnemyHealth))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected EnemyData data;
    [SerializeField] protected float globalPatrolRadius = 50f; // Радиус для выбора точек на всей карте
    [SerializeField] protected float searchTime = 3f;
    [SerializeField] protected float patrolWaitTime = 2f;
    [SerializeField] protected float minPatrolDistance = 2f;
    [SerializeField] protected float lookAroundAngle = 90f;
    [SerializeField] protected float lookAroundTime = 2f;

    protected NavMeshAgent navAgent;
    protected AnimationController animatorController;
    protected GameObject playerTarget;
    protected int currentHealth;
    protected Vector3 lastKnownPlayerPosition;
    protected float attackCooldownTimer;
    protected float searchTimer;
    protected float patrolWaitTimer;
    protected bool isAggro;
    protected bool isSearching;
    protected bool isInAttackCooldown;
    protected bool isWaitingAtPatrolPoint;
    protected bool isLookingAround;

    // Анимации
    protected virtual string IdleAnimation => "Idle";
    protected virtual string WalkAnimation => "Walk";
    protected virtual string AttackAnimation => "Attack";
    protected virtual string HitAnimation => "Hit";
    protected virtual string DeathAnimation => "Death";
    protected virtual string RunAnimation => "Run";

    protected virtual void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animatorController = GetComponent<AnimationController>();
        playerTarget = GameObject.FindGameObjectWithTag("Player");

        if (!navAgent || !animatorController || !playerTarget)
        {
            Debug.LogError("Missing required components or player reference!", this);
            enabled = false;
            return;
        }

        Initialize();
    }

    protected virtual void Initialize()
    {
        currentHealth = data.maxHealth;
        navAgent.speed = data.moveSpeed;
        navAgent.stoppingDistance = data.preferredDistance;
    }

    protected virtual void Update()
    {
        if (!IsAlive()) return;

        UpdateCooldowns();
        UpdateState();
        UpdateAnimations();
    }

    protected virtual void UpdateCooldowns()
    {
        if (isInAttackCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0)
            {
                isInAttackCooldown = false;
                navAgent.speed = isAggro ? data.moveSpeed * 1.5f : data.moveSpeed;
            }
        }

        if (isSearching)
        {
            searchTimer -= Time.deltaTime;
            if (searchTimer <= 0 && !isLookingAround)
            {
                StartCoroutine(LookAround());
            }
        }

        if (isWaitingAtPatrolPoint)
        {
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0)
            {
                isWaitingAtPatrolPoint = false;
                if (Random.value < 0.5f) // 50% шанс осмотра
                {
                    StartCoroutine(LookAround());
                }
                else
                {
                    Patrol();
                }
            }
        }
    }

    protected virtual void UpdateState()
    {
        if (DetectPlayer())
        {
            isAggro = true;
            isSearching = false;
            isWaitingAtPatrolPoint = false;
            isLookingAround = false;
            lastKnownPlayerPosition = playerTarget.transform.position;

            if (CanAttack())
            {
                OnAttack();
            }
            else
            {
                ChasePlayer();
            }
        }
        else if (isAggro && !isSearching && !isLookingAround)
        {
            StartSearch();
        }
        else if (!isAggro && !isWaitingAtPatrolPoint && !isLookingAround)
        {
            Patrol();
        }
    }

    protected virtual bool DetectPlayer()
    {
        if (!playerTarget) return false;

        float distance = Vector3.Distance(transform.position, playerTarget.transform.position);
        if (distance > data.chaseRange) return false;

        Vector3 directionToPlayer = (playerTarget.transform.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, directionToPlayer, out hit, data.chaseRange, LayerMask.GetMask("Default", "Player")))
        {
            return hit.transform.CompareTag("Player");
        }
        return false;
    }

    protected virtual void ChasePlayer()
    {
        navAgent.speed = data.moveSpeed * 1.5f;
        navAgent.SetDestination(playerTarget.transform.position);
    }

    protected virtual void OnAttack()
    {
        if (!isInAttackCooldown)
        {
            isInAttackCooldown = true;
            attackCooldownTimer = 10 / data.attackSpeed;
            navAgent.speed = data.moveSpeed * 0.5f;
            navAgent.SetDestination(transform.position);
            animatorController?.ChangeAnimation(AttackAnimation);
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (!IsAlive()) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        animatorController?.ChangeAnimation(HitAnimation);

        if (!IsAlive())
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        animatorController?.ChangeAnimation(DeathAnimation);
        navAgent.isStopped = true;
        Destroy(gameObject, 15f);
    }

    protected virtual void Patrol()
    {
        if (navAgent.hasPath && navAgent.remainingDistance > navAgent.stoppingDistance) return;

        // Случайно решаем, делать паузу или нет
        if (Random.value < 0.7f)
        {
            isWaitingAtPatrolPoint = true;
            patrolWaitTimer = patrolWaitTime;
            animatorController?.ChangeAnimation(IdleAnimation);
            return;
        }

        Vector3 newDestination;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            // Выбираем случайную точку на NavMesh в пределах globalPatrolRadius
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * globalPatrolRadius;
            randomPoint.y = transform.position.y; // Сохраняем высоту текущей позиции
            attempts++;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, globalPatrolRadius, NavMesh.AllAreas))
            {
                newDestination = hit.position;
                if (Vector3.Distance(newDestination, transform.position) >= minPatrolDistance)
                {
                    navAgent.speed = data.moveSpeed;
                    navAgent.SetDestination(newDestination);
                    animatorController?.ChangeAnimation(WalkAnimation);
                    return;
                }
            }
        } while (attempts < maxAttempts);

        // Если не удалось найти подходящую точку, ждем и пробуем снова
        isWaitingAtPatrolPoint = true;
        patrolWaitTimer = patrolWaitTime;
        animatorController?.ChangeAnimation(IdleAnimation);
    }

    protected virtual void StartSearch()
    {
        isSearching = true;
        searchTimer = searchTime;
        navAgent.speed = data.moveSpeed;
        navAgent.SetDestination(lastKnownPlayerPosition);
        animatorController?.ChangeAnimation(WalkAnimation);
    }

    protected virtual IEnumerator LookAround()
    {
        isLookingAround = true;
        animatorController?.ChangeAnimation(IdleAnimation);
        navAgent.isStopped = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + lookAroundAngle, 0);
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / (lookAroundTime * 0.5f);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 2 * lookAroundAngle, 0);
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / (lookAroundTime * 0.5f);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + lookAroundAngle, 0);
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / (lookAroundTime * 0.5f);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        isLookingAround = false;
        isSearching = false;
        isAggro = false;
        navAgent.isStopped = false;
        Patrol();
    }

    protected virtual void UpdateAnimations()
    {
        if (!IsAlive()) return;

        bool isMoving = navAgent.velocity.magnitude > 0.1f;

        if (isInAttackCooldown)
        {
            // Анимация атаки уже установлена
        }
        else if (isSearching && isMoving)
        {
            animatorController?.ChangeAnimation(WalkAnimation);
        }
        else if (isAggro && isMoving)
        {
            animatorController?.ChangeAnimation(RunAnimation);
        }
        else if (!isAggro && isMoving)
        {
            animatorController?.ChangeAnimation(WalkAnimation);
        }
        else
        {
            animatorController?.ChangeAnimation(IdleAnimation);
        }
    }

    public bool IsAlive() => currentHealth > 0;
    public Transform GetTransform() => transform;

    protected bool CanAttack()
    {
        if (!playerTarget) return false;
        float distance = Vector3.Distance(transform.position, playerTarget.transform.position);
        return distance <= data.attackRange && !isInAttackCooldown;
    }

    public void SetData(EnemyData newData)
    {
        data = newData;
        Initialize();
    }

    public EnemyData GetEnemyData()
    {
        return data;
    }
}