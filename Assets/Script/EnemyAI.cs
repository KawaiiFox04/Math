using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 8f;
    public float fieldOfView = 90f;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackFOV = 60f;
    public float attackCooldown = 1f;
    public int attackDamage = 1;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolWaitTime = 2f;
    public float patrolRadius = 5f;

    [Header("References")]
    public Transform player;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private Vector3 patrolTarget;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private float attackTimer = 0f;

    private NavMeshAgent agent;
    private PlayerHealth playerHealth;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player       = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();

            if (playerHealth == null)
                Debug.LogWarning("[EnemyAI] PlayerHealth component not found on Player!");
        }

        SetNewPatrolTarget();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            agent.isStopped = true;
            return;
        }

        attackTimer -= Time.deltaTime;

        if (CanAttackPlayer())
            currentState = State.Attack;
        else if (CanSeePlayer())
            currentState = State.Chase;
        else
            currentState = State.Patrol;

        switch (currentState)
        {
            case State.Patrol: HandlePatrol(); break;
            case State.Chase:  HandleChase();  break;
            case State.Attack: HandleAttack(); break;
        }
    }

    // ============================================================
    //  Detection — Dot Product (cone กว้าง)
    // ============================================================
    bool CanSeePlayer()
    {
        Vector3 toPlayerFlat = GetFlatDirectionToPlayer(out float distance);
        if (distance > detectionRange) return false;

        float dot        = Vector3.Dot(GetEnemyForward(), toPlayerFlat);
        float halfFovCos = Mathf.Cos(fieldOfView * 0.5f * Mathf.Deg2Rad);

        return dot >= halfFovCos;
    }

    // ============================================================
    //  Attack Detection — Dot Product (cone แคบ + ระยะใกล้)
    // ============================================================
    bool CanAttackPlayer()
    {
        Vector3 toPlayerFlat = GetFlatDirectionToPlayer(out float distance);
        if (distance > attackRange) return false;

        float dot           = Vector3.Dot(GetEnemyForward(), toPlayerFlat);
        float halfAtkFovCos = Mathf.Cos(attackFOV * 0.5f * Mathf.Deg2Rad);

        return dot >= halfAtkFovCos;
    }

    // ============================================================
    //  Patrol
    // ============================================================
    void HandlePatrol()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                SetNewPatrolTarget();
            }
            return;
        }

        agent.speed     = patrolSpeed;
        agent.isStopped = false;
        agent.SetDestination(patrolTarget);

        // รอให้ agent คำนวณ path เสร็จก่อนค่อยเช็ค
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            isWaiting       = true;
            waitTimer       = patrolWaitTime;
        }
    }

    void SetNewPatrolTarget()
    {
        // ลองสุ่มจุดใหม่จนกว่าจะได้จุดที่อยู่บน NavMesh จริงๆ
        for (int i = 0; i < 10; i++)
        {
            Vector2 rand      = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = transform.position + new Vector3(rand.x, 0, rand.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                return;
            }
        }

        // ถ้าสุ่ม 10 ครั้งไม่ได้เลย ให้อยู่กับที่ก่อน
        patrolTarget = transform.position;
    }

    // ============================================================
    //  Chase
    // ============================================================
    void HandleChase()
    {
        agent.speed     = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    // ============================================================
    //  Attack
    // ============================================================
    void HandleAttack()
    {
        agent.isStopped = true;
        FaceDirection(player.position - transform.position);

        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    void PerformAttack()
    {
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);

        Debug.Log($"[EnemyAI] Attacked Player for {attackDamage} damage!");
    }

    // ============================================================
    //  Helpers
    // ============================================================
    Vector3 GetEnemyForward()
    {
        return new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
    }

    Vector3 GetFlatDirectionToPlayer(out float distance)
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector3 flat     = new Vector3(toPlayer.x, 0, toPlayer.z);
        distance         = flat.magnitude;
        return distance > 0.001f ? flat / distance : Vector3.zero;
    }

    void FaceDirection(Vector3 direction)
    {
        direction.y = 0;
        if (direction.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    // ============================================================
    //  Gizmos
    // ============================================================
    void OnDrawGizmos()
    {
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 origin  = transform.position + Vector3.up * 0.1f;

        DrawCone(origin, forward, detectionRange, fieldOfView, Color.blue);
        DrawCone(origin, forward, attackRange, attackFOV, Color.red);
    }

    void DrawCone(Vector3 origin, Vector3 forward, float range, float fov, Color color)
    {
        Gizmos.color = color;

        Gizmos.DrawWireSphere(transform.position, range);

        float halfFov  = fov * 0.5f;
        int   segments = 20;

        Vector3 leftEdge  = Quaternion.Euler(0, -halfFov, 0) * forward * range;
        Vector3 rightEdge = Quaternion.Euler(0,  halfFov, 0) * forward * range;

        Gizmos.DrawLine(origin, origin + leftEdge);
        Gizmos.DrawLine(origin, origin + rightEdge);

        Vector3 prev = origin + leftEdge;
        for (int i = 1; i <= segments; i++)
        {
            float   angle = -halfFov + (fov / segments) * i;
            Vector3 next  = origin + Quaternion.Euler(0, angle, 0) * forward * range;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
}